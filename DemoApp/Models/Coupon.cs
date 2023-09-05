using DemoApp.Models.DTO;

namespace DemoApp.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float percent { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        //// AutoMapper do all the heavy lifting
        //public CouponDTO ToCouponDTO()
        //{
        //    return new CouponDTO
        //    {
        //        Id = this.Id,
        //        Name = this.Name,
        //        percent =  this.percent,
        //        IsActive = this.IsActive,
        //    };
        //}
    }

   
}
