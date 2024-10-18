using suud_site_cb.Utils;

var builder = WebApplication.CreateBuilder(args);

// set your site keys in this config section
builder.Services.Configure<SsoSuudClientSettings>(
    builder.Configuration.GetSection("SsoSuud")
);
builder.Services.AddSingleton<SsoSuudClient>();
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new TextPlainInputFormatter());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
