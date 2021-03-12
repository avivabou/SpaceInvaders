using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameObjects.FightTools;
using GameObjects.Screens;

namespace GameObjects.Managers
{
    public class IntersectionManager
    {
        private static readonly Color sr_transparent = Color.White * 0;
        private List<IHurtable> m_hurtables = new List<IHurtable>();
        private List<IAttacker> m_attackers = new List<IAttacker>();

        /// <summary>
        /// IntersectionManager constructor.
        /// </summary>
        /// <param name="i_game">The game that the manager live at.</param>
        public IntersectionManager(GameScreen i_GameScreen)
            : base()
        {
            i_GameScreen.ComponentAdded += components_ComponentAdded;
            i_GameScreen.ComponentRemoved += components_ComponentRemoved;
        }

        /// <summary>
        /// Check for intersection with the given IIntersectable.
        /// If there is an intersection with the given IIntersectable,
        /// and it can be hurt then it will be.
        /// </summary>
        /// <param name="i_Intersectable">IIntersectable object</param>
        public void CheckAndActForIntersection(IIntersectable i_Intersectable)
        {
            bool isAttacker = i_Intersectable is IAttacker;
            bool isHurtable = i_Intersectable is IHurtable;
            List<Vector2> intersectionPoints;

            for (int i = 0; i < m_hurtables.Count; i++)
            {
                if (m_hurtables[i].Souls > 0)
                {
                    if (pixelWiseIntersection(i_Intersectable, m_hurtables[i], out intersectionPoints))
                    {
                        if (isAttacker)
                        {
                            actForAttackerIntersection(i_Intersectable as IAttacker, m_hurtables[i]);
                        }
                        else if (isHurtable)
                        {
                            actForHurtableIntersection(i_Intersectable as IHurtable, m_hurtables[i], intersectionPoints);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Component removed from the game.
        /// Removing from the component from the manager.
        /// </summary>
        /// <param name="i_Sender">Object sender.</param>
        /// <param name="i_GameComponent">The component which removed.</param>
        private void components_ComponentRemoved(IGameComponent i_GameComponent)
        {
            if (i_GameComponent is IHurtable)
            {
                m_hurtables.Remove(i_GameComponent as IHurtable);
            }

            if (i_GameComponent is IAttacker)
            {
                m_attackers.Remove(i_GameComponent as IAttacker);
            }
        }

        /// <summary>
        /// Component add to the game.
        /// Adding the component to the manager.
        /// </summary>
        /// <param name="i_Sender">Object sender.</param>
        /// <param name="i_GameComponents">The component to add.</param>
        private void components_ComponentAdded(IGameComponent[] i_GameComponents)
        {
            for (int i = 0; i < i_GameComponents.Length; i++)
            {
                if (i_GameComponents[i] is IHurtable)
                {
                    m_hurtables.Add(i_GameComponents[i] as IHurtable);
                }

                if (i_GameComponents[i] is IAttacker)
                {
                    m_attackers.Add(i_GameComponents[i] as IAttacker);
                }
            }
        }

        /// <summary>
        /// Acting for attacker intersection.
        /// </summary>
        /// <param name="i_Attacker">The attacker object.</param>
        /// <param name="i_Hurtable">The object that will be hurt cause the attacker.</param>
        private void actForAttackerIntersection(IAttacker i_Attacker, IHurtable i_Hurtable)
        {
            if (i_Hurtable.HurtInCase.HasFlag(eHurtInCase.Attack))
            {
                if (i_Attacker.SelfTeam != i_Hurtable.SelfTeam)
                {
                    i_Attacker.Kill(i_Hurtable);
                }
            }
        }

        /// <summary>
        /// Acting for touching intersection.
        /// </summary>
        /// <param name="i_Touched">The touched object.</param>
        /// <param name="i_Toucher">The toucher object.</param>
        /// <param name="i_IntersectionPoints">All of the intersection points.</param>
        private void actForHurtableIntersection(IHurtable i_Touched, IHurtable i_Toucher, List<Vector2> i_IntersectionPoints)
        {
            if (i_Touched.HurtInCase.HasFlag(eHurtInCase.Touch))
            {
                if (i_Touched != i_Toucher)
                {
                    i_Touched.GotHurt(i_Toucher, i_IntersectionPoints);
                }
            }
        }

        /// <summary>
        /// Check for common pixels which are not transparent.
        /// </summary>
        /// <param name="i_Intersectable1">First IIntersectable object.</param>
        /// <param name="i_Intersectable2">Second IIntersectable object.</param>
        /// <param name="o_IntersectionPoints">All of the intersection points.</param>
        /// <returns>True if both objects intersecred, otherwise false.</returns>
        private bool pixelWiseIntersection(IIntersectable i_Intersectable1, IIntersectable i_Intersectable2, out List<Vector2> o_IntersectionPoints)
        {
            o_IntersectionPoints = new List<Vector2>();
            bool isIntersected = false;
            if (i_Intersectable1.ShapeRectangle.Intersects(i_Intersectable2.ShapeRectangle))
            {
                Rectangle intersectedRectangle = intersectionRectangle(i_Intersectable1, i_Intersectable2);
                Color[] sprite1Colors = i_Intersectable1.ObjectColors;
                Color[] sprite2Colors = i_Intersectable2.ObjectColors;
                for (int x = intersectedRectangle.Left; x <= intersectedRectangle.Right; x++)
                {
                    for (int y = intersectedRectangle.Bottom - 1; y >= intersectedRectangle.Top; y--)
                    {
                        int sprite1_index = x - i_Intersectable1.ShapeRectangle.Left + ((y - i_Intersectable1.ShapeRectangle.Top) * i_Intersectable1.ShapeRectangle.Width);
                        int sprite2_index = x - i_Intersectable2.ShapeRectangle.Left + ((y - i_Intersectable2.ShapeRectangle.Top) * i_Intersectable2.ShapeRectangle.Width);
                        if ((sprite1_index < sprite1Colors.Length) && (sprite2_index < sprite2Colors.Length))
                        {
                            Color currentPixelSprite1 = sprite1Colors[sprite1_index];
                            Color currentPixelSprite2 = sprite2Colors[sprite2_index];
                            if ((currentPixelSprite1 != sr_transparent) && (currentPixelSprite2 != sr_transparent))
                            {
                                isIntersected = true;
                            }

                            o_IntersectionPoints.Add(new Vector2(x, y));
                        }
                    }
                }
            }

            return isIntersected;
        }

        /// <summary>
        /// Rectangle boundries for available pixels that can be intersected.
        /// </summary>
        /// <param name="i_Intersectable1">First IIntersectable object.</param>
        /// <param name="i_Intersectable2">Second IIntersectable object.</param>
        /// <returns>A rectangle for available pixels that can be intersected.</returns>
        private Rectangle intersectionRectangle(IIntersectable i_Intersectable1, IIntersectable i_Intersectable2)
        {
            int max_x = MathHelper.Min(i_Intersectable1.ShapeRectangle.Right, i_Intersectable2.ShapeRectangle.Right);
            int min_x = MathHelper.Max(i_Intersectable1.ShapeRectangle.Left, i_Intersectable2.ShapeRectangle.Left);
            int max_y = MathHelper.Min(i_Intersectable1.ShapeRectangle.Bottom, i_Intersectable2.ShapeRectangle.Bottom);
            int min_y = MathHelper.Max(i_Intersectable1.ShapeRectangle.Top, i_Intersectable2.ShapeRectangle.Top);
            return new Rectangle(min_x, min_y, max_x - min_x, max_y - min_y);
        }
    }
}
