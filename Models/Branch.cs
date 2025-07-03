namespace AcrylicGame.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
        public ICollection<GalleryItem> GalleryItems { get; set; }
        public ICollection<Testimonial> Testimonials { get; set; }
    }
}
