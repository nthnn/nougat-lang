/*
	Copyright © 2022 Nathanne Isip

	Permission is hereby granted, free of charge,
	to any person obtaining a copy of this software
	and associated documentation files (the “Software”),
	to deal in the Software without restriction,
	including without limitation the rights to use, copy,
	modify, merge, publish, distribute, sublicense,
	and/or sell copies of the Software, and to permit
	persons to whom the Software is furnished to do so,
	subject to the following conditions:

	The above copyright notice and this permission notice
	shall be included in all copies or substantial portions
	of the Software.

	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF
	ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
	TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
	THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
	DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
	IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using Nougat.Core;
using Nougat.Parser;

public class NougatTokenizer : IAnalysis {
	private delegate bool BaseLexer(char Ch);

	private int Line;
	private int Column;
	private int Index;

	public readonly string FileName;
	public readonly string Source;

	public readonly List<Token> Tokens = new List<Token>();

	public NougatTokenizer(string FileName, string Source) {
		this.FileName = FileName;
		this.Source = Source;

		Line = (Column = 1);
		Index = 0;

		NougatUtil.DefineHash(NougatUtil.ToMD5(this.Source));
	}

	private bool IsAtEnd() {
		return Index == Source.Length;
	}

	private char Peek() {
		return Source[Index];
	}

	private char Previous() {
		return Source[Index - 1];
	}

	private char Consume() {
		Column++;
		return Source[Index++];
	}

	private void Scan() {
		while(!IsAtEnd()) {
			if(LexHelper.IsWhitespace(Peek())) {
				if(Peek() == '\n') {
					Line++;
					Column = 0;
				}

				Consume();
			}
			else if (LexHelper.IsIdentifier(Peek())) {
				int column = Column;
				string text = Consume().ToString();

				while (!IsAtEnd() && (LexHelper.IsIdentifier(Peek()) || LexHelper.IsDecimal(Peek())))
					text += Consume();
				Tokens.Add(Token.Create(Line, column, (!Constants.IsKeyword(text)) ? TokenType.IDENTIFIER : TokenType.KEYWORD, FileName, text));
			}
			else if (LexHelper.IsSymbol(Peek())) {
				if(Peek() == '#') {
					Consume();

					while (!IsAtEnd() && Peek() != '\n')
						Consume();
					Column = 1;
				}
				else if (Peek() == '"' || Peek() == '\'') {
					string text2 = "";
					int column2 = Column;
					char c = Consume();

					while (!IsAtEnd() && Peek() != c) {
						if(Peek() == '\n')
							throw new LexException(Line, column2, FileName, "String literals cannot contain new lines.");

						if(Peek() == '\\') {
							text2 = text2 + "[[" + Consume();

							if(!IsAtEnd()) {
								switch(Peek()) {
									case '"':
									case '\'':
									case '\\':
									case 'a':
									case 'f':
									case 'n':
									case 'r':
									case 's':
									case 't':
									case 'v':
										text2 = text2 + Consume() + "]]";
										break;
									case 'x': {
										string text6 = "";
										int num2 = 0;
										
										Consume();
										while(true) {
											if(IsAtEnd())
												throw new LexException(Line, Column, FileName, "Incomplete hexadecimal escape sequence.");

											if(!LexHelper.IsHexadecimal(Peek()))
												throw new LexException(Line, Column, FileName, "Expecting hexadecimal character.");

											text6 += Consume();
											if(num2 == 1)
												break;
											num2++;
										}

										text2 = text2.Substring(0, text2.Length - 1) + Convert.ToChar(int.Parse(text6, NumberStyles.HexNumber));
										break;
										}
									case 'u': {
										string text4 = "";
										int num = 0;
										
										Consume();
										while(true) {
											if(IsAtEnd())
												throw new LexException(Line, Column, FileName, "Incomplete hexadecimal escape sequence.");

											if(!LexHelper.IsHexadecimal(Peek()))
												throw new LexException(Line, Column, FileName, "Expecting hexadecimal character.");

											text4 += Consume();
											if(num == 3)
												break;
											num++;
										}

										text2 = text2.Substring(0, text2.Length - 1) + Convert.ToChar(long.Parse(text4, NumberStyles.HexNumber));
										break;
									}
									case 'b': {
										string text5 = "";
										Consume();

										while (!IsAtEnd() && LexHelper.IsBinary(Peek()))
											text5 += Consume();
										text2 = text2.Substring(0, text2.Length - 1) + Convert.ToChar(Convert.ToInt32(text5, 2));
										break;
									}
									case 'o': {
										string text3 = "";
										Consume();

										while (!IsAtEnd() && LexHelper.IsOctadecimal(Peek()))
											text3 += Consume();
										text2 = text2.Substring(0, text2.Length - 1) + Convert.ToChar(Convert.ToInt32(text3, 8));
										break;
									}
									default:
										throw new LexException(Line, Column, FileName, "Invalid character escape sequence: \\" + Peek());
								}
							}
						}
						else text2 += Consume();

						if (IsAtEnd())
							throw new LexException(Line, column2, FileName, "Unclosed string literal.");
					}

					Consume();
					Tokens.Add(Token.Create(Line, column2, TokenType.STRING, FileName, NougatUtil.Stringify(text2)));
				}
				else {
					int column3 = Column;
					string text7 = Consume().ToString();

					while (!IsAtEnd() && Constants.IsOperator(text7 + Peek()))
						text7 += Consume();
					Tokens.Add(Token.Create(Line, column3, TokenType.OPERATOR, FileName, text7));
				}
			}
			else {
				if(!LexHelper.IsDecimal(Peek()))
					continue;

				int column4 = Column;
				string text8 = Consume().ToString();

				if(Previous() == '0') {
					BaseLexer baseLexer = null;
					bool flag = false;

					if(IsAtEnd())
						throw new LexException(Line, column4, FileName, "Encountered end-of-file, expecting base code.");

					BaseLexer baseLexer2 = LexHelper.IsDecimal;
					switch(Peek()) {
						case 'b':
							baseLexer = LexHelper.IsBinary;
							break;
						case 'c':
							baseLexer = LexHelper.IsOctadecimal;
							break;
						case 'x':
							baseLexer = LexHelper.IsHexadecimal;
							break;
						default:
							baseLexer = baseLexer2;
							flag = true;
							break;
					}

					if(IsAtEnd())
						throw new LexException(Line, column4, FileName, "Encountered end-of-file, expecting base code.");

					if(!flag)
						text8 += Consume();

					while (!IsAtEnd() && baseLexer(Peek()))
						text8 += Consume();

					if(baseLexer == baseLexer2 && !IsAtEnd() && Peek() == '.') {
						text8 += Consume();

						while(!IsAtEnd() && baseLexer(Peek()))
							text8 += Consume();
					}

					Tokens.Add(Token.Create(Line, column4, TokenType.DIGIT, FileName, text8));
					continue;
				}

				while(!IsAtEnd() && LexHelper.IsDecimal(Peek()))
					text8 += Consume();

				if(!IsAtEnd() && Peek() == '.') {
					text8 += Consume();

					while(!IsAtEnd() && LexHelper.IsDecimal(Peek()))
						text8 += Consume();
				}

				Tokens.Add(Token.Create(Line, column4, TokenType.DIGIT, FileName, text8));
			}
		}
	}

	public override void Run() {
		try {
			Scan();
		}
		catch(LexException ex) {
			Errors.Add(ex.Locate(), ex.Message);
		}
	}

	public override bool IsSuccess() {
		return Errors.Count == 0;
	}
}