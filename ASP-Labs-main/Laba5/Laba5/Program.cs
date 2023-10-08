using Laba5;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"));
var app = builder.Build();

app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html;charset=utf-8";
    await context.Response.WriteAsync($"<div>");
    await context.Response.WriteAsync($"<p><a href='/form_add'>Додання даних</a></p>");
    await context.Response.WriteAsync($"<p><a href='/testing'>Перевірка даних</a></p>");
    await context.Response.WriteAsync($"</div>");

});

app.MapGet("/form_add", async context =>
{
    context.Response.ContentType = "text/html;charset=utf-8";
    await context.Response.WriteAsync($"<p>Додайте дані</p>");
    await context.Response.WriteAsync($"<form id=\"dataform\" action=\"/set_data\" method=\"post\">");
    await context.Response.WriteAsync($"<label for=\"valueInput\">Значення:</label>");
    await context.Response.WriteAsync($"<input type=\"text\" id=\"valueInput\" name=\"valueInput\" required><br>");
    await context.Response.WriteAsync($"<label for=\"dateTimeInput\">Дата і час (yyyy-MM-dd HH:mm:ss):</label>");
    await context.Response.WriteAsync($"<input type=\"datetime-local\" id=\"dateTimeInput\" name=\"dateTimeInput\" required><br>");
    await context.Response.WriteAsync($"<button type=\"submit\">Відправити</button>");
    await context.Response.WriteAsync($"</form>");

});
app.MapPost("/set_data", async context =>
{
    context.Response.ContentType = "text/html;charset=utf-8";
    var value = context.Request.Form["valueInput"];
    var Date = context.Request.Form["dateTimeInput"];
    if (DateTime.Parse(Date) < DateTime.Now)
    {
        await context.Response.WriteAsync($"<p>Значення \"{value}\"не було збережено, бо дата життя даних була вичерпана.</p>");
        await context.Response.WriteAsync($"<a href='/'>Додому</a> <br/>");
        await context.Response.WriteAsync("<a href='/form_add'>Додати нові дані</a>");
        throw new ApplicationException("Wrong destroy date for data");
    }
    if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(Date) && DateTime.TryParse(Date, out var destroy))
    {
        var options = new CookieOptions
        {
            Expires = destroy,
            IsEssential = true,
        };

        context.Response.Cookies.Append("datas", value, options);

        await context.Response.WriteAsync($"<p>Значення \"{value}\" було збережено.</p>");
        await context.Response.WriteAsync($"<a href='/'>Додому</a> <br/>");
        await context.Response.WriteAsync("<a href='/form_add'>Додати нові дані</a>");
    }
    else
    {
        await context.Response.WriteAsync("Помилка: Не вдалося зберегти дані");
        await context.Response.WriteAsync("<a href='/'>Додому</a>");
        await context.Response.WriteAsync("<a href='/form_add'>Додати нові дані</a>");
    }
});

app.MapGet("/testing", async context =>
{
    context.Response.ContentType = "text/html;charset=utf-8";
    if (context.Request.Cookies.TryGetValue("datas", out var value))
    {
        await context.Response.WriteAsync($"Данні: {value}.");
    }
    else
    {
        await context.Response.WriteAsync($"Немає збережених данних.");
        throw new ApplicationException("No data");
    }
});
app.Use(async (HttpContext context, RequestDelegate result) =>
{
    try
    {
        await result.Invoke(context);
    }
    catch (Exception exception)
    {
        var logger = app.Logger;
        var time = DateTime.Now.ToString();
        logger.LogError(time + " : " + exception.Message);
        throw;
    }
});
app.Run();