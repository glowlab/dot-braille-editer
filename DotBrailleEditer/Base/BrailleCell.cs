using System;
using System.Runtime.InteropServices;

namespace DotBrailleEditer.Base
{
    /// <summary>
    /// 表示一方盲文点字的结构。
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct BrailleCell : IComparable, IComparable<BrailleCell>, IEquatable<BrailleCell>
    {
        /// <summary>
        /// Unicode 点字快的首个字符。及空白盲文点位。
        /// </summary>
        public const char UnicodeFirstBrailleCharacter = '\u2800';

        /// <summary>
        /// 使用八个二进制位存储一方盲文点字中八个点状态的字段。
        /// </summary>
        private readonly byte value;

        /// <summary>
        /// 获取一个值， 表示没有任何点位突起的 BrailleCell 结构。此为静态字段。
        /// </summary>
        public static readonly BrailleCell EmptyCell = new BrailleCell(byte.MinValue);

        /// <summary>
        /// 获取一个值， 表示所有点位突起的 BrailleCell 结构。此为静态字段。
        /// </summary>
        public static readonly BrailleCell FallCell = new BrailleCell(byte.MaxValue);

        /// <summary>
        /// 获取一个 Byte 值， 表示一方盲文点字点的状态。
        /// </summary>
        public byte CellValue
        {
            get { return this.value; }
        }

        /// <summary>
        /// 获取一个使用 Unicode 盲文点字模型表示的盲文字符。
        /// </summary>
        public char Character
        {
            get { return (char)(UnicodeFirstBrailleCharacter + this.value); }
        }

        /// <summary>
        /// 使用一个整数值初始化 BrailleCell 结构。 整数值中只有 0 ~7 位的值会被使用，其余位会被忽略。
        /// </summary>
        /// <param name="cell">一个整数值， 表示盲文点字中八个点位的状态。</param>
        public BrailleCell(int cell)
        {
            this.value = (byte)cell;
        }

        /// <summary>
        /// 将此实例与另一实例进行比较， 并返回一个对二者相对值的指示。
        /// </summary>
        /// <param name="value">要比较的对象或 null。</param>
        /// <exception cref="System.ArgumentException">当 value 不是 BrailleCell 结构类型时候抛出此异常。</exception>
        /// <returns>返回一个小于 0、等于 0 或大于 0 的值，以只是该实例与另一实例的大小关系。</returns>
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is BrailleCell))
            {
                throw new ArgumentException("Argument must be BrailleCell type.");
            }

            return this.value - (((BrailleCell)value).value);
        }

        /// <summary>
        /// 将此实例与另一实例进行比较， 并返回一个对二者相对值的指示。
        /// </summary>
        /// <param name="value">要比较的对象。</param>
        /// <returns>返回一个小于 0、等于 0 或大于 0 的值，以只是该实例与另一实例的大小关系。</returns>
        public int CompareTo(BrailleCell value)
        {
            return this.value - value.value;
        }

        /// <summary>
        /// 创建一方突起指定盲文点位的 BrailleCell 实例。
        /// </summary>
        /// <param name="dotNumber">突起的点位。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当 dotNumber 的值不在 1 ~8 之间时引发此异常。</exception>
        /// <returns>返回 BrailleCell 实例值。</returns>
        public static BrailleCell Create(params int[] dotNumber)
        {
            int cell = new int();
            foreach (var num in dotNumber)
            {
                if (num < 1 || num > 8)
                {
                    throw new ArgumentOutOfRangeException(nameof(dotNumber), num, "Argument of value should be between 1 and 8.");
                }
                cell |= 1 << num - 1;
            }
            return new BrailleCell(cell);
        }

        /// <summary>
        /// 是否与另一对象值相等。
        /// </summary>
        /// <param name="obj">要比较的对象， 或为 null。</param>
        /// <returns>返回两对象是否相等的值。</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BrailleCell))
            {
                return false;
            }
            return this.value == ((BrailleCell)obj).value;
        }

        /// <summary>
        /// 比较该实例是否等于给定的实例值。
        /// </summary>
        /// <param name="value">要比较的实例。</param>
        /// <returns>返回两实例是否相等。</returns>
        public bool Equals(BrailleCell value)
        {
            return this.value == value.value;
        }

        /// <summary>
        /// 适用于本结构的哈希函数， 总是返回 CellValue 相同的值。
        /// </summary>
        /// <returns>返回该结构的哈希值。</returns>
        public override int GetHashCode()
        {
            return this.value;
        }

        /// <summary>
        /// 获取指定盲文点位的状态。
        /// </summary>
        /// <param name="dotNumber">1 ~8 之间的整数值， 指定盲文点位序号。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当 dotNumber 的值不在 1 ~8 之间时引发此异常。</exception>
        /// <returns>指定点位处于突起状态返回 true， 否则返回 false。</returns>
        public bool GetState(int dotNumber)
        {
            if (dotNumber < 1 || dotNumber > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(dotNumber), dotNumber, "Argument of value should be between 1 and 8.");
            }

            return (this.value & (1 << dotNumber - 1)) != 0;
        }

        /// <summary>
        /// 转成使用 Unicode 点字盲文符号表示各点位状态的字符串。
        /// </summary>
        /// <returns>返回 Unicode 盲文符号形式的字符串。</returns>
        public override string ToString()
        {
            return this.Character.ToString();
        }

        /// <summary>
        /// 把两方盲文点字突起的点合并为一方盲文点字。
        /// </summary>
        /// <param name="a">一方盲文点字。</param>
        /// <param name="b"><另一方盲文点字。/param>
        /// <returns>返回合并后一方的盲文点字。</returns>
        public static BrailleCell operator +(BrailleCell a, BrailleCell b) => new BrailleCell(a.value | b.value);

        /// <summary>
        /// 突起一方盲文点字中的指定点位。
        /// </summary>
        /// <param name="cell">被操作的盲文点字。</param>
        /// <param name="dotNumber">要抬起的点位编号。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当 dotNumber 的值不在 1 ~8 之间时引发此异常。</exception>
        /// <returns>返回抬起后的盲文点字。</returns>
        public static BrailleCell operator +(BrailleCell cell, int dotNumber)
        {
            if (dotNumber < 1 || dotNumber > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(dotNumber), dotNumber, "Argument of value should be between 1 and 8.");
            }
            return new BrailleCell(cell.value | 1 << dotNumber - 1);
        }

        /// <summary>
        /// 把一方盲文点字中那些在另外一方盲文点字中突起的点位抹平。
        /// </summary>
        /// <param name="a">被抹平的一方盲文点字。</param>
        /// <param name="b">另一方盲文点字。</param>
        /// <returns>返回抹平了指定点位后的一方盲文点字。</returns>
        public static BrailleCell operator -(BrailleCell a, BrailleCell b) => new BrailleCell(a.value & (b.value ^ 0XFF));

        /// <summary>
        /// 抹平一方盲文点字中的指定点位。
        /// </summary>
        /// <param name="cell">被操作的盲文点字。</param>
        /// <param name="dotNumber">要抹平的盲文点位编号。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当 dotNumber 的值不在 1 ~8 之间时引发此异常。</exception>
        /// <returns>返回抹平后的盲文点字。</returns>
        public static BrailleCell operator -(BrailleCell cell, int dotNumber)
        {
            if (dotNumber < 1 || dotNumber > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(dotNumber), dotNumber, "Argument of value should be between 1 and 8.");
            }
            return new BrailleCell(cell.value & (0XFF ^ (1 << dotNumber - 1)));
        }

        /// <summary>
        /// 比较两个实例值是否相等。
        /// </summary>
        /// <param name="a">一个盲文点字实例。</param>
        /// <param name="b">另一个盲文点字实例。</param>
        /// <returns>相等返回 true， 不相等返回 false。</returns>
        public static bool operator ==(BrailleCell a, BrailleCell b) => a.value == b.value;

        /// <summary>
        /// 比较两个实例值是否不相等。
        /// </summary>
        /// <param name="a">一个盲文点字实例。</param>
        /// <param name="b">另一个盲文点字实例。</param>
        /// <returns>不相等返回 true， 相等返回 false。</returns>
        public static bool operator !=(BrailleCell a, BrailleCell b) => a.value != b.value;
    }
}
