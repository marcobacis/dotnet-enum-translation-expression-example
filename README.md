# Enum translation from resx file using automapper

This is an example project to show how to use C#'s `Expression`s to translate enum values 
using Automapper and EF core (e.g. to search in a table using the enum translated text).

See the `EnumTranslatorExpression/EnumProjectionTranslationExtensions.cs` file for the helper method implementation, which creates a chain of conditionals to select the proper translation from the resx generated source.
