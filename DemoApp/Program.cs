using AutoMapper;
using DemoApp;
using DemoApp.Data;
using DemoApp.Models;
using DemoApp.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingConfig)); // Register our service

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.MapGet("/api/coupons", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting All Coupons"); // Dependency Injection
    return Results.Ok(CouponStore.CouponList);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);




app.MapGet("/api/coupon/{id:int}", (IMapper _mapper, int id) =>
{
    var coupon = CouponStore.CouponList.SingleOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        return Results.Ok($"Coupon with Id {id} not found");
    }

    return Results.Ok(_mapper.Map<CouponDTO>(coupon));

}).WithName("GetCoupon").Produces<CouponDTO>(200).Produces(400);




app.MapPost("/api/coupon", ([FromBody] CreateCouponDTO createCouponDTO, IMapper _mapper) =>
{
    if (string.IsNullOrEmpty(createCouponDTO.Name))
    {
        return Results.BadRequest("Name cannot be empty or null");
    }

    if (CouponStore.CouponList.FirstOrDefault(c => c.Name.ToLower() == createCouponDTO.Name) != null)
    {
        return Results.BadRequest($"Coupon with name {createCouponDTO.Name} already exist");
    }

    Coupon latestCoupon = CouponStore.CouponList.OrderByDescending(c => c.Id).FirstOrDefault();
    
    int newId = 1;
    if (latestCoupon != null) newId = latestCoupon.Id + 1;

    Coupon newCoupon = new Coupon
    {
        Id = newId,
        Name = createCouponDTO.Name,
        percent = createCouponDTO.percent,
        IsActive = createCouponDTO.IsActive,
        CreatedAt = DateTime.Now,
        LastUpdatedAt = DateTime.Now,
    };
    CouponStore.CouponList.Add(newCoupon);

    //return Results.Created($"/api/coupon/{coupon.Id}", coupon);

    return Results.CreatedAtRoute("GetCoupon", new { Id = newId }, _mapper.Map<CouponDTO>(newCoupon));

}).WithName("CreateCoupon").Accepts<CreateCouponDTO>("application/json").Produces<CouponDTO>(201);




app.MapPut("/api/coupon", () =>
{
});




app.MapDelete("/api/coupon/{id:int}", () =>
{
});


app.Run();
