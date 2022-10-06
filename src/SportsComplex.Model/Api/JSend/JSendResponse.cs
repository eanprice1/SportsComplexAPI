namespace SportsComplex.Model.Api.JSend
{
    public class JSendResponse
    {
        public string Status { get; set; } = JSendStatus.Success;
        public object Data { get; set; }
        public string Message { get; set; }
        public int? Code { get; set; }

        public JSendResponse()
        {
        }

        public JSendResponse(object data)
        {
            Data = data;
        }
    }
}
