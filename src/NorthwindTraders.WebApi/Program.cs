using FluentValidation;
using NorthwindTraders.WebApi.Data;
using NorthwindTraders.WebApi.Infrastructure.Conventions;
using NorthwindTraders.WebApi.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddRouting(opts => { opts.LowercaseUrls = true; });
builder.Services.AddControllers(opts =>
{
	opts.Conventions.Add(new ControllerTokenTransformerConvention());
	opts.Filters.Add<CustomExceptionFilterAttribute>();
	opts.Filters.Add<RequestAutoValidationFilter>(order: int.MinValue);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNorthwindTradersWebApi();
builder.Services.AddSwaggerGen(opts => { opts.DescribeAllParametersInCamelCase(); });
builder.Services.AddProblemDetails();

builder.Services.ConfigureData(builder.Configuration);

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
