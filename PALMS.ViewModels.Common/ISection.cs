namespace PALMS.ViewModels.Common
{
    public interface ISection
    {
        /// <summary>
        /// Gets the order index.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the name of section.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the section is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the section is enable.
        /// </summary>
        bool IsEnable { get; set; }

        /// <summary>
        /// Gets the image of section.
        /// </summary>
        string Image { get; }
    }
}