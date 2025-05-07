using Electronics;
using Electronics.Classes;
using Repository;
using Repository.Interface;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DeviceManager>(serviceProvider => {
    var configuration = builder.Configuration;
    string filePath = configuration["DeviceFilePath"] ?? "input.txt";
    return new DeviceManager(filePath);
});

builder.Services.AddSingleton<IDeviceService, DeviceService >(DeviceService =>
    new DeviceService(new DeviceRepository(connectionString))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();