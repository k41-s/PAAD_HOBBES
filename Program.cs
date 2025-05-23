var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider(); /*Maybe remove later*/
builder.Services.AddSingleton<PokemonGenerator.Services.PokemonService>();
builder.Services.AddSession(); /*maybe remove*/

var app = builder.Build();

app.UseSession(); /*maybe remove*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
