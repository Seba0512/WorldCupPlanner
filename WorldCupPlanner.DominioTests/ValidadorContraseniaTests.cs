using WorldCupPlanner.Dominio;

namespace WorldCupPlanner.DominioTests;

[TestClass]
public class ValidadorContraseniaTests
{
    // --- CumpleLongitud ---

    [TestMethod]
    public void CumpleLongitud_Con8Caracteres_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleLongitud("12345678"));
    }

    [TestMethod]
    public void CumpleLongitud_Con7Caracteres_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.CumpleLongitud("1234567"));
    }

    // --- CumpleMayuscula ---

    [TestMethod]
    public void CumpleMayuscula_ConMayuscula_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleMayuscula("Abc"));
    }

    [TestMethod]
    public void CumpleMayuscula_SinMayuscula_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.CumpleMayuscula("abc"));
    }

    // --- CumpleMinuscula ---

    [TestMethod]
    public void CumpleMinuscula_ConMinuscula_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleMinuscula("Abc"));
    }

    [TestMethod]
    public void CumpleMinuscula_SinMinuscula_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.CumpleMinuscula("ABC"));
    }

    // --- CumpleNumero ---

    [TestMethod]
    public void CumpleNumero_ConNumero_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleNumero("abc1"));
    }

    [TestMethod]
    public void CumpleNumero_SinNumero_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.CumpleNumero("abcD"));
    }

    // --- CumpleEspecial ---

    [TestMethod]
    public void CumpleEspecial_ConArrobaEspecial_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleEspecial("abc@"));
    }

    [TestMethod]
    public void CumpleEspecial_SinCaracterEspecial_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.CumpleEspecial("abcD1"));
    }

    [TestMethod]
    [DataRow("@")]
    [DataRow("#")]
    [DataRow("$")]
    [DataRow(".")]
    [DataRow("!")]
    [DataRow("&")]
    [DataRow("*")]
    [DataRow("-")]
    [DataRow("_")]
    [DataRow("+")]
    [DataRow("=")]
    [DataRow("?")]
    public void CumpleEspecial_ConCadaCaracterPermitido_RetornaTrue(string especial)
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.CumpleEspecial(especial));
    }

    // --- EsValida ---

    [TestMethod]
    public void EsValida_ConContraseniaQueReuneTodosLosRequisitos_RetornaTrue()
    {
        // Arrange & Act & Assert
        Assert.IsTrue(ValidadorContrasenia.EsValida("Pass@1234"));
    }

    [TestMethod]
    public void EsValida_ConContraseniaDe7Caracteres_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.EsValida("Pa@1234"));
    }

    [TestMethod]
    public void EsValida_SinMayuscula_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.EsValida("pass@1234"));
    }

    [TestMethod]
    public void EsValida_SinMinuscula_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.EsValida("PASS@1234"));
    }

    [TestMethod]
    public void EsValida_SinNumero_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.EsValida("Pass@abcd"));
    }

    [TestMethod]
    public void EsValida_SinCaracterEspecial_RetornaFalse()
    {
        // Arrange & Act & Assert
        Assert.IsFalse(ValidadorContrasenia.EsValida("Pass12345"));
    }
}
