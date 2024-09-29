using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using TestTask.Context;
using TestTask.Mapper;
using TestTask.Middleware;
using TestTask.Service;
using TestTask.Validator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderMapper, OrderMapper>();
builder.Services.AddHttpClient<IOrderService, OrderService>();

builder.Services.AddHangfire(configuration => 
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromSeconds(15),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

builder.Services.AddHangfireServer();

builder.Services.AddValidatorsFromAssemblyContaining<OrderCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<OrderUpdateDtoValidator>();

builder.Services.AddFluentValidationAutoValidation() 
    .AddFluentValidationClientsideAdapters(); 

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IOrderService>(
    "process_pending_orders", 
    service => service.ProcessPending(), 
    "*/5 * * * *"
);

RecurringJob.AddOrUpdate<IOrderService>(
    "process_processing_orders", 
    service => service.ProcessProcessingOrders(), 
    "*/5 * * * *"
);

RecurringJob.AddOrUpdate<IOrderService>(
    "recalculate_order_priorities", 
    service => service.RecalculatePriorities(), 
    "*/5 * * * *"
);



app.Run();