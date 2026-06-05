namespace Review_Guard.Domain.Servcies;

/// <summary>
/// Pure domain service — no dependencies, fully unit-testable.
/// 
/// Formula:
///   WeightedAvg = SUM(Rating × TrustScore) / SUM(TrustScore)
///
/// Only approved reviews are included.
/// Falls back to a simple average when all trust scores are 0.
/// </summary>
public static class WeightedAverageCalculator
{
    /// <summary>
    /// Calculates the weighted average rating from a set of approved review data.
    /// </summary>
    /// <param name="reviewWeights">
    ///   Sequence of (rating, trustScore) tuples for each approved review.
    /// </param>
    /// <returns>
    ///   Weighted average rounded to 2 decimal places, or 0 if no reviews exist.
    /// </returns>
    public static decimal Calculate(IEnumerable<(int Rating, decimal TrustScore)> reviewWeights)
    {
        var items = reviewWeights.ToList();
        if (items.Count == 0) return 0m;

        var totalWeight = items.Sum(r => r.TrustScore);

        // Guard: if all users somehow have TrustScore 0, fall back to simple average
        if (totalWeight == 0m)
            return Math.Round(items.Average(r => (decimal)r.Rating), 2);

        var weightedSum = items.Sum(r => r.Rating * r.TrustScore);

        return Math.Round(weightedSum / totalWeight, 2);
    }

    /// <summary>Convenience overload accepting separate parallel lists.</summary>
    public static decimal Calculate(IReadOnlyList<int> ratings, IReadOnlyList<decimal> trustScores)
    {
        if (ratings.Count != trustScores.Count)
            throw new ArgumentException("Ratings and trust scores must have the same length.");

        return Calculate(ratings.Select((r, i) => (r, trustScores[i])));
    }
}
