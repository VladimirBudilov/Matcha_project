namespace BLL.Helpers;

public class FameRatingCalculator
{
    public int Calculate(int likes, int views)
    {
        var likeWeight = 0.7;
        var viewWeight = 0.3;
        likes = likes * 100 / (likes + views);
        views = views * 100 / (likes + views);
        likes = (int)(likes * likeWeight);
        views = (int)(views * viewWeight); 
        return likes + views;
    }
}