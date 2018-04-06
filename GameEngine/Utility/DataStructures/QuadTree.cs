using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.Utility.DataStructures {
    public class QuadTree<T> {
        private readonly uint splitMargin;
        private readonly uint mergeMargin;
        private readonly (double x, double y, double X, double Y) bounds;
        private readonly QuadTree<T> parent;

        private Dictionary<T, (double x, double y)> items;

        private double splitX;
        private double splitY;
        private QuadTree<T>[] quads;

        public QuadTree(uint splitMargin)
            : this(splitMargin, 0, null, (double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity)) { }

        public QuadTree(uint splitMargin, uint mergeMargin)
            : this(splitMargin, mergeMargin, null, (double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity)) { }

        private QuadTree(uint splitMargin, uint mergeMargin, QuadTree<T> parent, (double x, double y, double X, double Y) bounds) {
            if (splitMargin < 2)
                throw new ArgumentOutOfRangeException(nameof(splitMargin), "The QuadTree split margin must be at least two.");

            if (mergeMargin >= splitMargin)
                throw new ArgumentOutOfRangeException(nameof(mergeMargin), "The merge margin must be smaller than the split margin.");

            this.splitMargin = splitMargin;
            this.mergeMargin = mergeMargin;
            this.bounds = (bounds.x, bounds.y, bounds.X, bounds.Y);
            this.parent = parent;

            this.splitX = float.NaN;
            this.splitY = float.NaN;

            this.items = new Dictionary<T, (double x, double y)>();

            this.quads = null;
        }

        public void Add(double x, double y, T item) {
            if (!Contains(x, y))
                parent?.Add(x, y, item);    // there is always a parent that contains it, since the root is infinity bound

            if (IsLeaf) {
                this.items[item] = (x, y);
            } else {
                int qIdx = ContainingQuadIndex(x, y);
                this.quads[qIdx].Add(x, y, item);
            }

            if (IsLeaf && Count >= this.splitMargin)
                Split();
        }

        public void Remove(T item) {
            if (IsLeaf) {
                this.items.Remove(item);
            } else {
                foreach (QuadTree<T> qt in this.quads)
                    qt.Remove(item);

                if (Count >= this.mergeMargin)
                    Merge();
            }
        }

        public void Move(double x, double y, T item) {
            Remove(item);
            Add(x, y, item);
        }

        public void ForEach(Action<T, double, double> action) {
            if (IsLeaf) {
                foreach (KeyValuePair<T, (double x, double y)> item in items) {
                    action(item.Key, item.Value.x, item.Value.y);
                }
            } else {
                foreach (QuadTree<T> qt in this.quads)
                    qt.ForEach(action);
            }
        }

        private void Split() {
            if (!IsLeaf)
                return;

            int halfItemCount = this.items.Count / 2;

            IOrderedEnumerable<KeyValuePair<T, (double x, double y)>> orderedX = this.items.OrderBy(pair => pair.Value.x);
            IOrderedEnumerable<KeyValuePair<T, (double x, double y)>> orderedY = this.items.OrderBy(pair => pair.Value.y);

            double splitX = float.NaN;
            double splitY = float.NaN;

            int i = 0;
            foreach (KeyValuePair<T, (double x, double y)> item in orderedX) {
                i++;

                if (i == halfItemCount) {
                    splitX = item.Value.x;
                } else if (i == halfItemCount + 1) {
                    splitX = splitX + (item.Value.x - splitX) / 2.0;
                    break;
                }
            }

            i = 0;
            foreach (KeyValuePair<T, (double x, double y)> item in orderedY) {
                i++;

                if (i == halfItemCount) {
                    splitY= item.Value.y;
                } else if (i == halfItemCount + 1) {
                    splitY = splitY + (item.Value.y - splitY) / 2.0;
                    break;
                }
            }

            this.splitX = splitX;
            this.splitY = splitY;

            this.quads = new[] {
                new QuadTree<T>(splitMargin, mergeMargin, this, (bounds.x, bounds.y, splitX, splitY)),
                new QuadTree<T>(splitMargin, mergeMargin, this, (bounds.x, splitY, splitX, bounds.Y)),
                new QuadTree<T>(splitMargin, mergeMargin, this, (splitX, bounds.y, bounds.X, splitY)),
                new QuadTree<T>(splitMargin, mergeMargin, this, (splitX, splitY, bounds.X, bounds.Y))
            };

            foreach (KeyValuePair<T, (double x, double y)> item in this.items) {
                bool inHalfX = item.Value.x < splitX;
                bool inHalfY = item.Value.y < splitY;

                if (inHalfX && inHalfY)
                    this.quads[0].Add(item.Value.x, item.Value.y, item.Key);
                else if (inHalfX)
                    this.quads[1].Add(item.Value.x, item.Value.y, item.Key);
                else if (inHalfY)
                    this.quads[2].Add(item.Value.x, item.Value.y, item.Key);
                else
                    this.quads[3].Add(item.Value.x, item.Value.y, item.Key);
            }

            this.items = null;
        }

        private void Merge() {
            if (IsLeaf)
                return;

            foreach (QuadTree<T> quad in this.quads)    // not neccessary since Split() is private, but what the heck, maybe it'll go public one day and than this is nice to have
                quad.Merge();
            
            this.items = new Dictionary<T, (double x, double y)>();
            IEnumerable<KeyValuePair<T, (double x, double y)>> items = this.quads.SelectMany(qt => qt.items);
            foreach (KeyValuePair<T, (double x, double y)> item in items)
                this.items[item.Key] = item.Value;

            this.quads = null;
            this.splitX = float.NaN;
            this.splitY = float.NaN;
        }

        private int ContainingQuadIndex(double x, double y) {
            if (IsLeaf || !Contains(x, y))
                return -1;

            bool inHalfX = x < this.splitX;
            bool inHalfY = y < this.splitY;

            if (inHalfX && inHalfY)
                return 0;

            if (inHalfX)
                return 1;

            if (inHalfY)
                return 2;

            return 3;
        }

        public IEnumerable<T> ItemsIn(double x, double y, double width, double height) {
            if (IsLeaf) {
                return this.items.Where(item => item.Value.x >= x && item.Value.x <= x + width && item.Value.y >= y && item.Value.y <= y + height).Select(item => item.Key);
            } else {
                return this.quads
                    .Where(tmp => tmp.Contains(x, y) || tmp.Contains(x + width, y) || tmp.Contains(x, y + height) || tmp.Contains(x + width, y + height))
                    .SelectMany(qt => qt.ItemsIn(x, y, width, height));
            }
        }

        public IEnumerable<T> ItemsIn(double x, double y, double radius) {
            double r2 = radius * radius;

            if (IsLeaf) {
                return this.items.Where(item => {
                    double dx = x - item.Value.x;
                    double dy = y - item.Value.y;

                    return dx * dx + dy * dy <= r2;
                }).Select(item => item.Key);
            } else {
                double xMin = x - radius;
                double yMin = y - radius;
                double width = 2f * radius;
                double height = 2f * radius;

                return this.quads
                    .Where(tmp => tmp.Contains(xMin, yMin) || tmp.Contains(xMin + width, yMin) || tmp.Contains(xMin, yMin + height) || tmp.Contains(xMin + width, yMin + height))
                    .SelectMany(qt => qt.ItemsIn(x, y, width, height));
            }
        }

        public IEnumerable<T> Items => IsLeaf ? this.items.Keys : this.quads.SelectMany(qt => qt.Items);

        public bool Contains(double x, double y) => x >= this.bounds.x && x < this.bounds.X && y >= this.bounds.y && y < this.bounds.Y;

        public int Count => IsLeaf ? this.items.Count : this.quads.Sum(t => t.Count);

        public bool IsLeaf => this.quads == null;
    }
}
