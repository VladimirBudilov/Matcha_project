namespace BLL.Helpers;

public static class FameRatingCalculator
{
    public static int Calculate(int likes, int views)
    {
        if (views == 0) return 0;

        var rating = (double)likes / views * 100;
        return (int)rating; 
    }
}