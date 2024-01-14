using System.Text;
using System.Text.RegularExpressions;

namespace ECommerceShop.Helper
{
    
    public static class Utilities
    {
        public static bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static int PAGE_SIZE = 20;
        public static void CreateIfMissing(string path)
        {
            bool folderExist = Directory.Exists(path);
            if (!folderExist) {
            Directory.CreateDirectory(path);
            }
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    var s = words[i];   
                    if (s.Length > 0)
                    {
                        words[i] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static bool IsInteger(string str)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            try
            {
                if (String.IsNullOrEmpty(str))
                {
                    return false;
                }
                if (!regex.IsMatch(str))
                {
                    return false;
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public static string GetRamdomKey(int length = 5)
        {
            string pattern = @"01123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạảãâấầậẫẩăắằặẵẳ]", "a");
            url = Regex.Replace(url, @"[éèẹẻẽêếềệểễ]", "e");
            url = Regex.Replace(url, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            url = Regex.Replace(url, @"[úùụủũưứừựửữ]", "u");
            url = Regex.Replace(url, @"[íìịỉĩ]", "i");
            url = Regex.Replace(url, @"[ýỳỵỷỹ]", "y");
            url = Regex.Replace(url, @"[đ]", "d");
            url = Regex.Replace(url.Trim(), @"[^a-z0-9-\s]", "").Trim();
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                    CreateIfMissing(path);
                    string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newname);
                    var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };

                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                        return null;
                }
                    else
                    {
                        using (var stream = new FileStream(pathFile, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        return newname;
                    }
                
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}
