using UnityEngine;
using System.Collections;

public static class BloxValidationErrorMessages
{

    public const string INT_BLOX_NO_VALUE = "O bloco INT é de preenchimento obrigatório";
    public const string BOOL_BLOX_NO_VALUE = "O bloco BOOL é de preenchimento obrigatório";
    public const string STRING_BLOX_NO_VALUE = "O bloco STRING é de preenchimento obrigatório";
    public const string FOR_BLOX_NO_VALUES = "O bloco for possui valores não preenchidos";

    public const string IF_BLOX_NO_VAR = "O bloco IF não possui uma variável ou um operador lógico associado";
    public const string IF_BLOX_NO_CHILDREN = "O bloco IF não possui blocos filhos";
    public const string FOR_BLOX_NO_CHILDREN = "O bloco FOR não possui blocos filhos";

    public const string INT_BLOX_NO_NAME = "O bloco INT não possui nome";
    public const string BOOL_BLOX_NO_NAME = "O bloco BOOL não possui nome";
    public const string STRING_BLOX_NO_NAME = "O bloco STRING não possui nome";
    public const string FOR_BLOX_NO_NAME = "O contador de FOR não possui nome";

    public const string OPERATOR_BLOX_NO_VALUE = "O bloco de operador não possui todos os valores preenchidos";

    public const string BLOX_REPEATED_NAME = "Já existe no mesmo scope um bloco com o mesmo nome";

    public const string OBSTACLE_AHEAD = "Não é possível mover em frente pois existe um obstáculo.";
    public const string NO_OBSTACLE_AHEAD = "Não é possível subir para um obstáculo inexistente.";
    public const string HOLE_AHEAD_EXCEPTION = "Não é possível andar em frente para um buraco.";
    public const string NO_HOLE_AHEAD_EXCEPTION = "Não é possível descer para um Tile que não se encontra em nível inferior.";
    public const string NO_TILE_AHEAD_EXCEPTION = "Não é possível mover para um Tile inexistente.";
    public const string DIVISION_BY_ZERO_EXCEPTION = "Não é possível dividir por zero.";


    public const string VARIABLE_CHOICE_BLOX_NO_VALUE = "Não foi seleccionada uma variável.";
    public const string PRINT_BLOX_NO_PARAMS = "O bloco PRINT não tem parâmetros associados.";


}
