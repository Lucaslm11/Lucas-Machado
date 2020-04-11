using UnityEngine;
using System.Collections;

public static class LevelCompletionMessages
{
    public const string MAIN_ERROR_TEXT = "A execução terminou com as seguintes falhas:\n";
    public const string ERROR_MANDATORY_STEPS = "\tNão realização dos passos marcados na grelha\n";
    public const string ERROR_MANDATORY_BLOXES = "\tNão utilização dos blocos obrigatórios\n";
    public const string WRONG_STEPS= "\tNúmero de passos errados: {0}\n";


    public const string MAIN_SUCCESS_TEXT = "A execução terminou com sucesso.\n";
    public const string MAIN_OBJECTIVES = "\tObjectivos principais: 1000 pontos \n";
    public const string OPTIONAL_BLOXES = "\tBlocos opcionais ({0}/{1}): {2} / 1000 pontos \n";
    public const string TIME = "\tTempo excedido ({0}): {1} / 1000 pontos \n";
    public const string LINES = "\tNúmero de linhas de código ({0}/{1}): {2} / 1000 pontos \n";
    public const string ATTEMPTS = "\tNúmero de tentativas ({0}/{1}): {2} / 1000 pontos \n";
    public const string SUM = "\tSoma: {0} pontos \n";
    public const string STARS = "\t{0} estrelas\n";

}
