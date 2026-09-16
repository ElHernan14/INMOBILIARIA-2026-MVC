namespace INMOBILIARIA.Models
{
	public class PagedResults<T>
	{
		public List<T> Resultados {get; set;} = [];
		public int TotalResults {get; set;}
		public int CurrentPage {get; set;}
		public int PageSize {get; set;}
		public int TotalPages => (int)Math.Ceiling((double)TotalResults / PageSize);
	}
}