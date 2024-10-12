using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yare.DataAccess.Repository.IRepository;
using Yare.Models;

public class DynamicPricingService : BackgroundService
{
    private readonly ILogger<DynamicPricingService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DynamicPricingService(ILogger<DynamicPricingService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Dynamic Pricing Service is starting.");

        var nonBestSellingDelay = TimeSpan.FromDays(7); // 14 days (2 weeks)
        var nextNonBestSellingRun = DateTime.UtcNow.Add(nonBestSellingDelay);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Update prices for best-selling products every 10 minutes
                    UpdateBestSellingProductPrices(unitOfWork);

                    // Check if it's time to update non-best-selling products
                    if (DateTime.UtcNow >= nextNonBestSellingRun)
                    {
                        UpdateNonBestSellingProductPrices(unitOfWork);
                        nextNonBestSellingRun = DateTime.UtcNow.Add(nonBestSellingDelay); // Schedule the next run
                    }
                }

                // Wait for 10 minutes before the next iteration
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Dynamic Pricing Service.");
            }
        }

        _logger.LogInformation("Dynamic Pricing Service is stopping.");
    }

    private async Task UpdateBestSellingProductPrices(IUnitOfWork unitOfWork)
    {
        _logger.LogInformation("Updating prices for best-selling products.");

        string collectionName = "Best Sellers";
        int topCount = 80;

        var products = unitOfWork.product.GetAll();
        var collections = unitOfWork.Collection.GetAll();
        var orderDetails = unitOfWork.OrderDetail.GetAll();

        var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == collectionName);
        if (bestSellersCollection == null) return;

        int bestSellersCollectionId = bestSellersCollection.Id;

        var bestSellingProducts = orderDetails
            .GroupBy(od => od.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalCount = g.Sum(od => od.Count),
                TotalQuantity = g.Sum(od => od.Count * od.Price)
            })
            .OrderByDescending(x => x.TotalCount)
            .Take(topCount)
            .ToList();

        var bestSellersList = bestSellingProducts
            .Join(products,
                  bs => bs.ProductId,
                  p => p.Id,
                  (bs, p) => new { bs.ProductId, Product = p, bs.TotalCount, bs.TotalQuantity })
            .ToList();

        foreach (var bestSeller in bestSellersList)
        {
            var product = bestSeller.Product;
            var originalPrice = product.Price;

            var newPrice = Math.Round(originalPrice * 1.10, 2);
            product.PriceWas = Math.Round(originalPrice, 2);
            product.Price = newPrice;           
            product.TargetPrice01 = Math.Round(newPrice - (newPrice * 0.05), 2);
            product.TargetPrice02 = Math.Round(newPrice - (newPrice * 0.10), 2);
            product.TargetPrice03 = Math.Round(newPrice - (newPrice * 0.25), 2);

            unitOfWork.product.Update(product);
        }

        unitOfWork.Save();
    }

    private async Task UpdateNonBestSellingProductPrices(IUnitOfWork unitOfWork)
    {
        _logger.LogInformation("Updating prices for non-best-selling products.");

        string collectionName = "Best Sellers";

        var products = unitOfWork.product.GetAll();
        var productCollections = unitOfWork.Product_Collection.GetAll();
        var collections = unitOfWork.Collection.GetAll();

        var bestSellersCollection = collections.FirstOrDefault(c => c.CollectionName == collectionName);
        if (bestSellersCollection == null) return;

        int bestSellersCollectionId = bestSellersCollection.Id;

        var nonBestSellersList = products
            .Where(p => !productCollections.Any(pc => pc.ProductId == p.Id && pc.CollectionId == bestSellersCollectionId))
            .ToList();

        foreach (var product in nonBestSellersList)
        {
            var originalPrice = product.Price;

            // Adjust price based on the target prices
            if (product.RemainigQuantity <= 50)
            {
                product.Price = Math.Round(product.TargetPrice03, 2);
            }
            else if (product.RemainigQuantity <= 100)
            {
                product.Price = Math.Round(product.TargetPrice02, 2);
            }
            else
            {
                product.Price = Math.Round(product.TargetPrice01, 2);
            }

            product.PriceWas = Math.Round(originalPrice, 2); // Save the original price rounded to two decimal places
            unitOfWork.product.Update(product);
        }

        unitOfWork.Save();
    }
}

