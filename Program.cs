using Farma_api.Dependencies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Inject(builder.Configuration);
builder.Services.InjectAuth(builder.Configuration);
builder.Services.InjectDocumentation();
builder.Services.InjectCors();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseStaticFiles();
    app.UseHsts();
}

app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowSpecificOrigin"); 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
if (app.Environment.IsProduction())
    app.MapFallbackToFile("index.html");
app.Run();