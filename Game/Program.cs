using AethelgardGame.Models.Game;
using AethelgardGame.Models.Story;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<StoryLibrary>();
builder.Services.AddSingleton<GameEngine>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// hình, nhạc, js, css: trình duyệt phải hỏi lại máy chủ mỗi lần (trả 304 nếu chưa đổi), để file thay mới cùng tên hiện ra ngay
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = c => c.Context.Response.Headers["Cache-Control"] = "no-cache",
});
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
