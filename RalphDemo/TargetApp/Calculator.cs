namespace TargetApp;

public class Calculator
{
    // Bug 1: Should be Addition
    public int Add(int a, int b)
    {
        return a - b; 
    }

    // Bug 2: Should be Multiplication
    public int Multiply(int a, int b)
    {
        return a + b;
    }
}
