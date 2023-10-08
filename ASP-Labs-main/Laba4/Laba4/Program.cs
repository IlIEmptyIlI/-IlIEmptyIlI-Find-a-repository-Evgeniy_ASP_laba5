using Laba4.Classes;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Configuration.AddJsonFile("config.json");
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync($"<div>");
    await context.Response.WriteAsync($"<p><a href='/library'>library</a></p>");
    await context.Response.WriteAsync($"<p><a href='/library/books'>List of books</a></p>");
    await context.Response.WriteAsync($"</div>");
});
app.MapGet("library/profiles/{id:int?}", async (int? id, IConfiguration appConfig, HttpContext context) => {
    Profile[] profiles = appConfig.GetSection("Lib:Profiles").Get<Profile[]>();
    await context.Response.WriteAsync("<div>");
    if (id.HasValue && profiles != null && id.Value >= 0 && id.Value < profiles.Length)
    { await context.Response.WriteAsync($"<p>Firstname : {profiles[id.Value].Firstname}, Lastname : {profiles[id.Value].Lastname}<p>"); }
    else { await context.Response.WriteAsync($"<p>Name : {context.User}, identity : {context.User.Identity}<p>"); }
    await context.Response.WriteAsync("</div>");
}
);
app.MapGet("library", async (HttpContext context) =>
{
    string salutation = "This is the library! You can find some books here";
    await context.Response.WriteAsync($"<div>{salutation}</div>");
});
app.MapGet("library/books", async (HttpContext context, IConfiguration appConfig) =>
{
    Book[] books = appConfig.GetSection("Lib:Books").Get<Book[]>();
    await context.Response.WriteAsync("<div>");
    foreach (var item in books)
    {
        await context.Response.WriteAsync($"<p>Name of book : {item.Name}, its author : {item.Author}<p>");
    }
    await context.Response.WriteAsync("</div>");
});
app.Run();