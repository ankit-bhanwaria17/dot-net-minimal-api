namespace DemoApp.Models.DTO
{
    public class UpdateCouponDTO
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public float percent { get; set; }
        public bool IsActive { get; set; }
    }
}
