using System;

namespace ConsoleControl
{
  /// <summary>
  /// The ConsoleStreamEventArgs are arguments for a console event.
  /// </summary>
  public class ConsoleStreamEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    public ConsoleStreamEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    public ConsoleStreamEventArgs(string content)
    {
        //  Set the content and code.
        Content = content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    /// <param name="code">The code.</param>
    public ConsoleStreamEventArgs(int code)
    {
        //  Set the content and code.
        Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEventArgs"/> class.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="code">The code.</param>
    public ConsoleStreamEventArgs(string content, int code)
    {
      //  Set the content and code.
      Content = content;
      Code = code;
    }

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Content
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>
    /// The code.
    /// </value>
    public int? Code
    {
        get;
        private set;
    }
  }
}
