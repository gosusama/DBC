namespace BTS.API.SERVICE.Helper
{ 
    public class RoleState
    {
        public string STATE { get; set; }
        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Approve { get; set; }
        public bool Giamua { get; set; }
        public bool Giaban { get; set; }
        public bool Giavon { get; set; }
        public bool Tylelai { get; set; }
        public bool Banchietkhau { get; set; }
        public bool Banbuon { get; set; }
        public bool Bantralai { get; set; }

        public RoleState()
        {
            STATE = "";
            View = false;
            Add = false;
            Edit = false;
            Delete = false;
            Approve = false;
            Giamua = false;
            Giaban = false;
            Giavon = false;
            Tylelai = false;
            Banchietkhau = false;
            Banbuon = false;
            Bantralai = false;
        }
    }
}
