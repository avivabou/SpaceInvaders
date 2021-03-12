using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameObjects.GameProperties;

namespace GameObjects.Screens
{
    public class MenuCollection : DrawableGameComponent
    {
        private GameSettings m_gameSettings;
        private MenuLabel[] m_menuLabels;
        private Rectangle m_screenBoundries;
        private float m_linesGapProportion;
        private int m_selected = -1;

        public int Selected
        {
            get
            {
                return m_selected;
            }

            set
            {
                if (m_selected != -1)
                {
                    m_menuLabels[m_selected].IsSelected = false;
                }

                if (value == MathHelper.Clamp(value, 0, m_menuLabels.Length))
                {
                    m_menuLabels[value].IsSelected = true;
                    m_selected = value;
                }
            }
        }

        public int Count
        {
            get
            {
                return m_menuLabels.Length;
            }
        }

        /// <summary>
        /// MenuCollection constructor.
        /// </summary>
        /// <param name="i_Game">Game.</param>
        /// <param name="i_MenuStrings">Array of strings which will present as Menu Labels.</param>
        /// <param name="i_ScreenBoundries">Screen bounderies for collection.</param>
        /// <param name="i_LinesGapProportion">Gap between lines as a scale of line.</param>
        public MenuCollection(Game i_Game, string[] i_MenuStrings, Rectangle? i_ScreenBoundries = null, float i_LinesGapProportion = 1)
            : base(i_Game)
        {
            m_menuLabels = new MenuLabel[i_MenuStrings.Length];
            for (int i = 0; i < m_menuLabels.Length; i++)
            {
                m_menuLabels[i] = new MenuLabel(Game, i_MenuStrings[i]);
            }

            m_linesGapProportion = i_LinesGapProportion - 1;
            if (i_ScreenBoundries.HasValue)
            {
                m_screenBoundries = i_ScreenBoundries.Value;
            }
            else
            {
                m_screenBoundries = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            }
        }

        /// <summary>
        /// Initialize MenuCollection.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            fitTextToScreen();
            m_gameSettings = Game.Services.GetService(typeof(GameSettings)) as GameSettings;
        }

        /// <summary>
        /// Update any MenuLabel in collection.
        /// </summary>
        /// <param name="i_GameTime">Game Time.</param>
        public override void Update(GameTime i_GameTime)
        {
            if (Enabled)
            {
                for (int i = 0; i < m_menuLabels.Length; i++)
                {
                    if (m_menuLabels[i].IsSelected)
                    {
                        m_selected = i;
                    }

                    m_menuLabels[i].Update(i_GameTime);
                }
            }
        }

        /// <summary>
        /// Draw any MenuLabel in collection.
        /// </summary>
        /// <param name="i_GameTime"></param>
        public override void Draw(GameTime i_GameTime)
        {
            foreach (MenuLabel label in m_menuLabels)
            {
                label.Draw(i_GameTime);
            }
        }

        /// <summary>
        /// Get the menu label at the given index.
        /// </summary>
        /// <param name="i_Index">The index of MenuLabel.</param>
        /// <returns>The MenuLabel at the given index.</returns>
        public MenuLabel GetMenuLabelAt(int i_Index)
        {
            return m_menuLabels[i_Index];
        }

        /// <summary>
        /// Set a new boundries to menu collection.
        /// </summary>
        /// <param name="i_Boundries"></param>
        public void SetRectangleBoundries(Rectangle i_Boundries)
        {
            m_screenBoundries = i_Boundries;
            fitTextToScreen();
        }

        /// <summary>
        /// Fitting text to center of screen boundries.
        /// </summary>
        private void fitTextToScreen()
        {
            float sectionProportion, averageHeight, minY;
            getLabelesLocationParams(out sectionProportion, out averageHeight, out minY);
            placeLabels(sectionProportion, averageHeight, minY);
        }

        /// <summary>
        /// Sum up the total height of labels and find the max width.
        /// </summary>
        /// <param name="o_TotalHeight">Total height (sum of all labels heights).</param>
        /// <param name="o_MaxWidth">Max width of all labels.</param>
        private void calcTotalLabelsSize(out float o_TotalHeight, out float o_MaxWidth)
        {
            o_TotalHeight = 0;
            o_MaxWidth = 0;
            foreach (MenuLabel label in m_menuLabels)
            {
                label.Initialize();
                o_TotalHeight += label.Measuring.Y * 2;
                o_MaxWidth = MathHelper.Max(o_MaxWidth, label.Measuring.X);
            }
        }

        /// <summary>
        /// Calcing the proportion of all labels.
        /// </summary>
        /// <param name="o_SectionProportion">Proportion of text.</param>
        /// <param name="o_AverageHeight">Average height of all labels.</param>
        /// <param name="o_MinY">Min Y to start printing from.</param>
        private void getLabelesLocationParams(out float o_SectionProportion, out float o_AverageHeight, out float o_MinY)
        {
            float totalHeight, maxWidth;
            calcTotalLabelsSize(out totalHeight, out maxWidth);
            float totalSections = (m_menuLabels.Length * 2) + 1;
            o_AverageHeight = totalHeight / m_menuLabels.Length;
            totalHeight += (m_menuLabels.Length - 1) * o_AverageHeight * m_linesGapProportion;
            o_SectionProportion = 0.95f * m_screenBoundries.Height / totalHeight;
            float maxProportion = m_screenBoundries.Width / (1.05f * maxWidth);
            o_SectionProportion = MathHelper.Clamp(o_SectionProportion, 0, maxProportion);
            totalHeight *= o_SectionProportion;
            o_AverageHeight *= o_SectionProportion;
            o_MinY = (0.05f * m_screenBoundries.Height) + ((m_screenBoundries.Height - totalHeight) / 2);
        }

        /// <summary>
        /// Placing all labels by the given paramters.
        /// </summary>
        /// <param name="i_SectionProportion">Proportion of text.</param>
        /// <param name="i_AverageHeight">Average height of all labels.</param>
        /// <param name="i_MinY">Min Y to start printing from.</param>
        private void placeLabels(float i_SectionProportion, float i_AverageHeight, float i_MinY)
        {
            for (int i = 0; i < m_menuLabels.Length; i++)
            {
                Vector2 location;
                float textLength = m_menuLabels[i].Measuring.X * i_SectionProportion;
                location.X = (m_screenBoundries.Width - textLength) / 2;
                location.X += m_screenBoundries.X;
                location.Y = i_MinY + ((i * i_AverageHeight) * (1 + m_linesGapProportion));
                location.Y += m_screenBoundries.Y;
                m_menuLabels[i].SetProportion(location, i_SectionProportion);
            }
        }
    }
}
