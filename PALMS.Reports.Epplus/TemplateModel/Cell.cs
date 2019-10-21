namespace PALMS.Reports.Epplus.TemplateModel
{
    public struct Cell
    {
        public int Row { get; }
        public int Col { get; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}