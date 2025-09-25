namespace ApexMagnamentFrontend.DTOs
{
    public class PaginatedResponse<T>
    {
        public List<T> Content { get; set; } = new();
        // Si quieres, puedes agregar otras propiedades como totalPages, pageNumber, etc.
    }
}
