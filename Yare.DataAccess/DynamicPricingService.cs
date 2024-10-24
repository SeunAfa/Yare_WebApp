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

        var nonBestSellingDelay = TimeSpan.FromDays(25); // 25 days for non-best-selling products
        var nextNonBestSellingRun = DateTime.UtcNow.Add(nonBestSellingDelay);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Update prices for best-selling products every hour
                    UpdateBestSellingProductPrices(unitOfWork);

                    // Check if it's time to update non-best-selling products
                    if (DateTime.UtcNow >= nextNonBestSellingRun)
                    {
                        UpdateNonBestSellingProductPrices(unitOfWork);
                        nextNonBestSellingRun = DateTime.UtcNow.Add(nonBestSellingDelay); // Schedule the next run
                    }
                }

                // Wait for 6 hour before the next iteration for best-selling products
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
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

            // Directly use the pre-calculated TargetPrice01, TargetPrice02, TargetPrice03
            if (product.RemainigQuantity <= 50)
            {
                product.Price = product.TargetPrice03; // Lowest price for low stock
            }
            else if (product.RemainigQuantity <= 100)
            {
                product.Price = product.TargetPrice02; // Medium price for medium stock
            }
            else
            {
                product.Price = product.TargetPrice01; // Highest price for higher stock
            }

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
            // Assign prices based on remaining quantity without recalculating the target prices
            if (product.RemainigQuantity <= 50)
            {
                product.Price = product.TargetPrice03; // Lowest price
            }
            else if (product.RemainigQuantity <= 100)
            {
                product.Price = product.TargetPrice02; // Medium price
            }
            else
            {
                product.Price = product.TargetPrice01; // Highest price
            }

            product.PriceWas = product.Price; // Keep track of the previous price if needed
            unitOfWork.product.Update(product);
        }

        unitOfWork.Save();
    }

}

