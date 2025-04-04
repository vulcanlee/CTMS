namespace CTMS.Services;

public class RequestInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestInfoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetFullUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return "Unknown";

        var scheme = request.Scheme; // http 或 https
        var host = request.Host.Host; // 主機名稱
        var port = request.Host.Port ?? 80; // 埠號 (預設 80 或 443)

        return $"{scheme}://{host}:{port}";
    }
}