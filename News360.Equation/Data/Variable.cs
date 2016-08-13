namespace News360.Equation.Data
{
    /// <summary>
    /// Represents single variable entrance
    /// </summary>
    public class Variable
    {
        public string Name { get; set; }
        public int Power { get; set; }

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Object" />.</summary>
        public Variable(string name, int power)
        {
            Name = name;
            Power = power;
        }
    }
}