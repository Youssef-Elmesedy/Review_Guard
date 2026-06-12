Select * From Users
where FullName = 'Nour Essam';

SELECT
    COUNT(*) AS TotalReviews,
    SUM(OverallRating) AS TotalRating,
    AVG(OverallRating) AS AverageRating
FROM Reviews
WHERE UserId = 'B337496F-DAA0-4268-8EDB-D33A0AEEDF62';