namespace AgencyBanking_UI.Model
{
    /// <summary>
    /// A menu item used to hold data.
    /// </summary>
   public  class MenuItem
    {
        public string Title { get; }
        public string Imagelink { get; }
        public string Actioname { get; }

        public MenuItem(string title, string imagelink, string actioname)
        {
            Title = title;
            Imagelink = imagelink;
            Actioname = actioname;
        }

    }
}
