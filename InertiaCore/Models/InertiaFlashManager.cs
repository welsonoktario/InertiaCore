using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

internal class InertiaFlashMessages
{
    private readonly ISession? session;

    public InertiaFlashMessages(ISession? session)
    {
        this.session = session;
    }

    private IDictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();

    public IDictionary<string, object?> GetData() => Data;

    public void Set(string key, object? value)
    {
        Data[key] = value;

        session?.SetString("flash", JsonSerializer.Serialize(Data));
    }

    public void Merge(Dictionary<string, string>? with)
    {
        if (with is null)
            return;

        foreach (var (key, value) in with)
        {
            Data[key] = value;
        }

        session?.SetString("flash", JsonSerializer.Serialize(Data));
    }

    public void Clear(bool clearData = true)
    {
        if (clearData)
            Data.Clear();

        session?.Remove("flash");
    }

    public static InertiaFlashMessages FromSession(HttpContext httpContext)
    {
        try
        {
            var flash = new InertiaFlashMessages(httpContext.Session);
            var sessionData = httpContext.Session.GetString("flash");
            if (sessionData is not null)
            {
                flash.Data = JsonSerializer.Deserialize<IDictionary<string, object?>>(sessionData) ?? new Dictionary<string, object?>();
            }

            return flash;
        }
        catch (InvalidOperationException)
        {
            return new InertiaFlashMessages(null);
        }

    }
}
