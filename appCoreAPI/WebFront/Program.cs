var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// IMPRESCINDIBLE: Configurar la Sesión (porque la usas en tu Login)
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // La sesión dura 30 minutos
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Login}/{id?}");



app.Run();