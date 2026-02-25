using System;
using System.Collections.Generic;

namespace ReCal.Models
{
    public class CraftingResult
    {
        public int CansNeeded { get; set; }
        public int CageLightsNeeded { get; set; }
        public int ScrapProduced { get; set; }
        public int ScrapLeftover { get; set; }
        public int WireProduced { get; set; }
        public int WireLeftover { get; set; }
        public int MetalBarsProduced { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CostPerLocker { get; set; }
    }

    public class ProfitCalculation
    {
        public decimal SellingPrice { get; set; }
        public decimal ProfitPerLocker { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class LockerCalculator
    {
        public CraftingResult CalculateMaterials(int lockerCount)
        {
            // Raw recipe data
            int scrapPerLocker = 38;
            int wirePerLocker = 8;
            int scrapPerCan = 4;
            decimal canCost = 59m;
            int wirePerCageLight = 2;
            int metalBarsPerCageLight = 2;
            decimal cageLightCost = 69m;

            // Calculate total requirements
            int totalScrapNeeded = scrapPerLocker * lockerCount;
            int totalWireNeeded = wirePerLocker * lockerCount;

            // Calculate cans needed
            int cansNeeded = (int)Math.Ceiling((double)totalScrapNeeded / scrapPerCan);
            int scrapProduced = cansNeeded * scrapPerCan;
            int scrapLeftover = scrapProduced - totalScrapNeeded;
            decimal cansCost = cansNeeded * canCost;

            // Calculate cage lights needed
            int cageLightsNeeded = (int)Math.Ceiling((double)totalWireNeeded / wirePerCageLight);
            int wireProduced = cageLightsNeeded * wirePerCageLight;
            int wireLeftover = wireProduced - totalWireNeeded;
            int metalBarsProduced = cageLightsNeeded * metalBarsPerCageLight;
            decimal cageLightsCost = cageLightsNeeded * cageLightCost;

            decimal totalCost = cansCost + cageLightsCost;
            decimal costPerLocker = totalCost / lockerCount;

            return new CraftingResult
            {
                CansNeeded = cansNeeded,
                CageLightsNeeded = cageLightsNeeded,
                ScrapProduced = scrapProduced,
                ScrapLeftover = scrapLeftover,
                WireProduced = wireProduced,
                WireLeftover = wireLeftover,
                MetalBarsProduced = metalBarsProduced,
                TotalCost = totalCost,
                CostPerLocker = costPerLocker
            };
        }

        public List<ProfitCalculation> CalculateProfitStrategies(int lockerCount, decimal costPerLocker, decimal marketPrice)
        {
            var strategies = new List<ProfitCalculation>
            {
                new ProfitCalculation { SellingPrice = marketPrice },
                new ProfitCalculation { SellingPrice = marketPrice * 0.85m },
                new ProfitCalculation { SellingPrice = marketPrice * 0.75m },
                new ProfitCalculation { SellingPrice = marketPrice * 0.65m }
            };

            foreach (var strategy in strategies)
            {
                strategy.ProfitPerLocker = strategy.SellingPrice - costPerLocker;
                strategy.ProfitMargin = (strategy.ProfitPerLocker / strategy.SellingPrice) * 100;
                strategy.TotalRevenue = strategy.SellingPrice * lockerCount;
                strategy.TotalProfit = strategy.ProfitPerLocker * lockerCount;
            }

            return strategies;
        }
    }
}
