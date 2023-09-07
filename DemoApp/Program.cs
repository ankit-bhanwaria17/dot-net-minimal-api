using AutoMapper;
using DemoApp;
using DemoApp.Data;
using DemoApp.Models;
using DemoApp.Models.DTO;
using DemoApp.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingConfig)); // Register our service
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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
    APIResponse response = new APIResponse();
    response.Result = CouponStore.CouponList;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    _logger.Log(LogLevel.Information, "Getting All Coupons"); // Dependency Injection
    return Results.Ok(response);

}).WithName("GetCoupons").Produces<IEnumerable<APIResponse>>(200);




app.MapGet("/api/coupon/{id:int}", (IMapper _mapper, int id) =>
{
    APIResponse response = new APIResponse();
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    var coupon = CouponStore.CouponList.SingleOrDefault(c => c.Id == id);
    if (coupon == null)
    {
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages.Add($"Coupon with Id {id} not found");
        return Results.BadRequest(response);
    }
    response.Result = _mapper.Map<CouponDTO>(coupon);

    return Results.Ok(response);

}).WithName("GetCoupon").Produces<CouponDTO>(200).Produces(400);




app.MapPost("/api/coupon", async ([FromBody] CreateCouponDTO createCouponDTO, IMapper _mapper, IValidator<CreateCouponDTO> _validation) =>
{
    APIResponse response = new APIResponse();
    response.IsSuccess = false;
    response.StatusCode = HttpStatusCode.BadRequest;

    //var validationResult = _validation.ValidateAsync(createCouponDTO).GetAwaiter().GetResult();

    var validationResult = await _validation.ValidateAsync(createCouponDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    if (CouponStore.CouponList.FirstOrDefault(c => c.Name.ToLower() == createCouponDTO.Name) != null)
    {
        response.ErrorMessages.Add($"Coupon with name {createCouponDTO.Name} already exist");
        return Results.BadRequest(response);
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

    response.Result = _mapper.Map<CouponDTO>(newCoupon);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

    //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
    //return Results.CreatedAtRoute("GetCoupon", new { Id = newId }, _mapper.Map<CouponDTO>(newCoupon));

}).WithName("CreateCoupon").Accepts<CreateCouponDTO>("application/json").Produces<APIResponse>(201);




app.MapPut("/api/coupon", async ([FromBody] UpdateCouponDTO updateDTO, IMapper _mapper, IValidator<UpdateCouponDTO> _validator) =>
{
    APIResponse response = new APIResponse();
    response.IsSuccess = false;
    response.StatusCode = HttpStatusCode.BadRequest;
    var validation = await _validator.ValidateAsync(updateDTO);

    if (!validation.IsValid)
    {
        response.ErrorMessages.Add(validation.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    var coupon = CouponStore.CouponList.FirstOrDefault(c => c.Id == updateDTO.Id);
    if (coupon == null)
    {
        response.ErrorMessages.Add($"Coupon with Id = {updateDTO.Id} not found");
        return Results.BadRequest(response);
    }

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    coupon.Name = updateDTO.Name;
    coupon.percent = updateDTO.percent;
    coupon.LastUpdatedAt = DateTime.Now;

    response.Result = coupon;

    return Results.Ok(response);
   
}).Accepts<UpdateCouponDTO>("application/json").Produces<APIResponse>(201);




app.MapDelete("/api/coupon/{id:int}", (int id, IMapper _mapper) =>
{
    APIResponse response = new APIResponse() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };
    var couponIndex = CouponStore.CouponList.FindIndex(c => c.Id == id);
    if (couponIndex == -1)
    {
        response.ErrorMessages.Add($"Coupon with Id = {id} not found");
        return Results.NotFound(response);
    }
    response.Result = CouponStore.CouponList[couponIndex];
    CouponStore.CouponList.RemoveAt(couponIndex);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).Produces<APIResponse>();

app.Run();
