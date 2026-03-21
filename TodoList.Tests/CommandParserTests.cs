using Xunit;
using TodoList;
using TodoList.Exceptions;

namespace TodoList.Tests;

public class CommandParserTests
{

	[Fact]
	public void Parse_AddCommand_WithText_ReturnsAddCommand()
	{
		// Arrange
		string input = "add Buy milk";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var add = Assert.IsType<AddCommand>(cmd);
		Assert.Equal("Buy milk", add.Text);
		Assert.False(add.Multiline);
	}

	[Fact]
	public void Parse_AddCommand_WithMultilineFlag_ReturnsAddCommandWithMultiline()
	{
		// Arrange
		string input = "add -m";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var add = Assert.IsType<AddCommand>(cmd);
		Assert.True(add.Multiline);
	}

	[Fact]
	public void Parse_StatusCommand_ValidArgs_ReturnsStatusCommand()
	{
		// Arrange
		string input = "status 5 Completed";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var status = Assert.IsType<StatusCommand>(cmd);
		Assert.Equal(5, status.Id);
		Assert.Equal(TodoStatus.Completed, status.NewStatus);
	}

	[Fact]
	public void Parse_StatusCommand_InvalidId_Throws()
	{
		// Arrange
		string input = "status abc Completed";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}

	[Fact]
	public void Parse_StatusCommand_InvalidStatus_Throws()
	{
		// Arrange
		string input = "status 1 WrongStatus";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}

	[Fact]
	public void Parse_UpdateCommand_ValidArgs_ReturnsUpdateCommand()
	{
		// Arrange
		string input = "update 3 New text";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var update = Assert.IsType<UpdateCommand>(cmd);
		Assert.Equal(3, update.Id);
		Assert.Equal("New text", update.NewText);
	}

	[Fact]
	public void Parse_UpdateCommand_MissingText_Throws()
	{
		// Arrange
		string input = "update 3";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}

	[Theory]
	[InlineData("delete 2")]
	[InlineData("remove 2")]
	[InlineData("rm 2")]
	public void Parse_RemoveCommands_ReturnsRemoveCommand(string input)
	{
		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var remove = Assert.IsType<RemoveCommand>(cmd);
		Assert.Equal(2, remove.Id);
	}

	[Fact]
	public void Parse_RemoveCommand_InvalidId_Throws()
	{
		// Arrange
		string input = "delete abc";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}


	[Fact]
	public void Parse_ViewCommand_FlagsParsedCorrectly()
	{
		// Arrange
		string input = "view -i -s -d";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var view = Assert.IsType<ViewCommand>(cmd);
		Assert.True(view.ShowIndex);
		Assert.True(view.ShowStatus);
		Assert.True(view.ShowUpdateDate);
		Assert.False(view.ShowAll);
	}

	[Fact]
	public void Parse_ViewCommand_UnknownFlag_Throws()
	{
		// Arrange
		string input = "view -x";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}

	[Fact]
	public void Parse_SearchCommand_AllFlagsParsedCorrectly()
	{
		// Arrange
		string input =
			"search --contains milk --starts-with Buy --ends-with now " +
			"--from 2024-01-01 --to 2024-12-31 --status Completed " +
			"--sort text --desc --top 5";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var search = Assert.IsType<SearchCommand>(cmd);

		Assert.Equal("milk", search.ContainsText);
		Assert.Equal("Buy", search.StartsWithText);
		Assert.Equal("now", search.EndsWithText);
		Assert.Equal(new DateTime(2024, 1, 1), search.FromDate);
		Assert.Equal(new DateTime(2024, 12, 31), search.ToDate);
		Assert.Equal(TodoStatus.Completed, search.Status);
		Assert.Equal("text", search.SortBy);
		Assert.True(search.Desc);
		Assert.Equal(5, search.Top);
	}

	[Fact]
	public void Parse_SearchCommand_UnknownFlag_Throws()
	{
		// Arrange
		string input = "search --unknown value";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}

	[Fact]
	public void Parse_LoadCommand_ValidArgs_ReturnsLoadCommand()
	{
		// Arrange
		string input = "load 3 100";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var load = Assert.IsType<LoadCommand>(cmd);
		Assert.Equal(3, load.DownloadCount);
		Assert.Equal(100, load.DownloadSize);
	}

	[Fact]
	public void Parse_LoadCommand_InvalidArgs_Throws()
	{
		// Arrange
		string input = "load a b";

		// Act & Assert
		Assert.Throws<InvalidArgumentException>(() => CommandParser.Parse(input));
	}


	[Fact]
	public void Parse_ProfileCommand_LogoutFlag_ReturnsProfileCommand()
	{
		// Arrange
		string input = "profile -o";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var profile = Assert.IsType<ProfileCommand>(cmd);
		Assert.True(profile.IsLogout);
		Assert.False(profile.IsSwitching);
	}

	[Fact]
	public void Parse_ProfileCommand_SwitchFlag_ReturnsProfileCommand()
	{
		// Arrange
		string input = "profile --switch";

		// Act
		var cmd = CommandParser.Parse(input);

		// Assert
		var profile = Assert.IsType<ProfileCommand>(cmd);
		Assert.True(profile.IsSwitching);
		Assert.False(profile.IsLogout);
	}

	[Fact]
	public void Parse_HelpCommand_ReturnsHelpCommand()
	{
		// Act
		var cmd = CommandParser.Parse("help");

		// Assert
		Assert.IsType<HelpCommand>(cmd);
	}

	[Fact]
	public void Parse_UndoCommand_ReturnsUndoCommand()
	{
		// Act
		var cmd = CommandParser.Parse("undo");

		// Assert
		Assert.IsType<UndoCommand>(cmd);
	}

	[Fact]
	public void Parse_RedoCommand_ReturnsRedoCommand()
	{
		// Act
		var cmd = CommandParser.Parse("redo");

		// Assert
		Assert.IsType<RedoCommand>(cmd);
	}

	[Fact]
	public void Parse_EmptyInput_Throws()
	{
		Assert.Throws<InvalidCommandException>(() => CommandParser.Parse(""));
	}

	[Fact]
	public void Parse_UnknownCommand_Throws()
	{
		Assert.Throws<InvalidCommandException>(() => CommandParser.Parse("unknowncmd"));
	}
}
