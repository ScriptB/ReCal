using System;

namespace ReCal.Models
{
    public class CigarCalculator
    {
        // Constants
        private const decimal CIGAR_PRICE = 100m;
        private const int CIGARS_PER_HOUR = 300;
        private const int TOBACCO_PER_CIGAR = 2;
        private const int CLOTH_PER_CIGAR = 2;
        private const int TOBACCO_PER_PLANT = 6;
        private const int CLOTH_PER_TAPE = 3;

        public CigarCalculationResult CalculateCigars(int cigarCount)
        {
            if (cigarCount <= 0 || cigarCount > 10000)
            {
                throw new ArgumentException("Cigar count must be between 1 and 10000");
            }

            var result = new CigarCalculationResult();

            // Calculate outputs
            result.CigarCount = cigarCount;
            result.TotalRevenue = cigarCount * CIGAR_PRICE;
            result.RevenuePerCigar = CIGAR_PRICE;

            // Calculate production time
            result.ProductionHours = (decimal)cigarCount / CIGARS_PER_HOUR;
            result.ProductionMinutes = result.ProductionHours * 60;
            result.ProductionSeconds = result.ProductionHours * 3600;

            // Calculate exact inputs (fractional)
            result.TobaccoNeeded = cigarCount * TOBACCO_PER_CIGAR;
            result.ClothNeeded = cigarCount * CLOTH_PER_CIGAR;
            result.PlantsNeededExact = (decimal)result.TobaccoNeeded / TOBACCO_PER_PLANT;
            result.TapeNeededExact = (decimal)result.ClothNeeded / CLOTH_PER_TAPE;

            // Calculate whole item inputs (rounded up)
            result.PlantsNeeded = (int)Math.Ceiling(result.PlantsNeededExact);
            result.TapeNeeded = (int)Math.Ceiling(result.TapeNeededExact);

            // Calculate leftovers
            result.TobaccoLeftover = (result.PlantsNeeded * TOBACCO_PER_PLANT) - result.TobaccoNeeded;
            result.ClothLeftover = (result.TapeNeeded * CLOTH_PER_TAPE) - result.ClothNeeded;

            // Calculate hourly metrics
            result.CigarsPerHour = CIGARS_PER_HOUR;
            result.RevenuePerHour = CIGARS_PER_HOUR * CIGAR_PRICE;
            result.RevenuePerMinute = result.RevenuePerHour / 60;
            result.RevenuePerSecond = result.RevenuePerHour / 3600;

            // Calculate hourly input requirements
            result.PlantsPerHour = 100; // 300 cigars * 2 tobacco / 6 tobacco per plant
            result.TapePerHour = 200; // 300 cigars * 2 cloth / 3 cloth per tape

            // Calculate efficiency metrics
            result.CostPerCigar = 0; // No material costs in this calculation
            result.ProfitPerCigar = CIGAR_PRICE;
            result.ProfitMargin = 100; // 100% profit margin since no costs

            return result;
        }

        public CigarBatchResult CalculateBatch(CigarBatchSize batchSize)
        {
            int cigarCount = GetCigarCountForBatch(batchSize);
            return new CigarBatchResult
            {
                BatchSize = batchSize,
                CigarCount = cigarCount,
                Revenue = cigarCount * CIGAR_PRICE,
                PlantsNeeded = (int)Math.Ceiling((decimal)(cigarCount * TOBACCO_PER_CIGAR) / TOBACCO_PER_PLANT),
                TapeNeeded = (int)Math.Ceiling((decimal)(cigarCount * CLOTH_PER_CIGAR) / CLOTH_PER_TAPE)
            };
        }

        private int GetCigarCountForBatch(CigarBatchSize batchSize)
        {
            switch (batchSize)
            {
                case CigarBatchSize.Ten:
                    return 10;
                case CigarBatchSize.Hundred:
                    return 100;
                case CigarBatchSize.Hourly:
                    return CIGARS_PER_HOUR;
                default:
                    return 1;
            }
        }
    }

    public class CigarCalculationResult
    {
        public int CigarCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenuePerCigar { get; set; }
        public decimal ProductionHours { get; set; }
        public decimal ProductionMinutes { get; set; }
        public decimal ProductionSeconds { get; set; }

        // Material requirements
        public int TobaccoNeeded { get; set; }
        public int ClothNeeded { get; set; }
        public decimal PlantsNeededExact { get; set; }
        public decimal TapeNeededExact { get; set; }
        public int PlantsNeeded { get; set; }
        public int TapeNeeded { get; set; }

        // Leftovers
        public int TobaccoLeftover { get; set; }
        public int ClothLeftover { get; set; }

        // Hourly metrics
        public int CigarsPerHour { get; set; }
        public decimal RevenuePerHour { get; set; }
        public decimal RevenuePerMinute { get; set; }
        public decimal RevenuePerSecond { get; set; }
        public int PlantsPerHour { get; set; }
        public int TapePerHour { get; set; }

        // Efficiency metrics
        public decimal CostPerCigar { get; set; }
        public decimal ProfitPerCigar { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    public class CigarBatchResult
    {
        public CigarBatchSize BatchSize { get; set; }
        public int CigarCount { get; set; }
        public decimal Revenue { get; set; }
        public int PlantsNeeded { get; set; }
        public int TapeNeeded { get; set; }
    }

    public enum CigarBatchSize
    {
        Ten,
        Hundred,
        Hourly
    }
}
