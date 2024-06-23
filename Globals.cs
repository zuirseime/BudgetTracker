namespace BudgetTracker;

public static class Globals
{
    public const string AdminRole = "Admin";
    public const string UserRole = "User";
    public const string ImageFolder = "wwwroot/content/images";
    public const string DefaultPicture = "ImagePlaceholder.png";

    public static string GetUserPic(string image, bool truncate = true) {
        if (truncate) {
            int firstSlash = ImageFolder.IndexOf('/');
            return $"{ImageFolder[firstSlash..]}/{image}";
        }
        else return $"{ImageFolder}/{image}";
    }
}