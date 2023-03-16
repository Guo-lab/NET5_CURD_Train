using Microsoft.AspNetCore.Http;

namespace ESC5.WebCommon
{
    /// <summary>
    /// @author Rainy
    /// </summary>
    public interface IBaseSessionHelper
    {

        bool HasFormToken(HttpRequest request, string vmTypeName, string token);

        void SaveFormToken(HttpRequest request, string vmTypeName, string token);

        int GetLoginWrongCnt(HttpRequest request);

        void AddLoginWrongCnt(HttpRequest request);

        bool HasFormToken(HttpRequest request);

    }
}