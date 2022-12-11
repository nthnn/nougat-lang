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

using System.Collections.Generic;
using Nougat.Core;
using Nougat.Parser;
using Nougat.Parser.AST;
using Nougat.Parser.AST.Expr;
using Nougat.Parser.AST.Stmt;

public class NougatParser : IAnalysis {
	private List<Token> Tokens;
	private int Index;

	public readonly List<Statement> Globals = new List<Statement>();

	public NougatParser(List<Token> Tokens) {
		this.Tokens = Tokens;
	}

	private static string KindToString(TokenType Kind) {
		return Kind switch {
			TokenType.DIGIT => "number", 
			TokenType.IDENTIFIER => "identifier", 
			TokenType.OPERATOR => "operator", 
			TokenType.KEYWORD => "keyword", 
			TokenType.STRING => "string literal", 
			_ => null, 
		};
	}

	private Token Current() {
		return Tokens[Index];
	}

	private bool IsAtEnd() {
		return Index == Tokens.Count;
	}

	private Token LookAhead(int Count) {
		return Tokens[Index + Count];
	}

	private bool LookAhead(string[] Tokens) {
		foreach(string text in Tokens)
			if(LookAhead().Image == text)
				return true;

		return false;
	}

	private Token LookAhead() {
		return LookAhead(0);
	}

	private bool CanLookAhead(int Count) {
		return Index + 1 <= Tokens.Count;
	}

	private Token Consume(TokenType Kind) {
		if(IsAtEnd()) {
			Errors.Add(Current().Locate(), "Expecting " + Kind.ToString().ToLower() + ", encountered end-of-file.");
			throw new ParserException();
		}

		Token token = LookAhead();
		if(token.Kind == Kind) {
			Index++;
			return token;
		}

		Errors.Add(Tokens[Index].Locate(), "Expecting " + KindToString(Kind) + ", encountered " + KindToString(Tokens[Index].Kind) + ". (" + Tokens[Index].FileName + ")");
		throw new ParserException();
	}

	private void Consume(string Image) {
		if(LookAhead().Image == Image) {
			Index++;
			return;
		}

		Errors.Add(Tokens[Index].Locate(), "Expecting \"" + Image + "\", encountered \"" + Tokens[Index].Image + "\". (" + Tokens[Index].FileName + ")");
		throw new ParserException();
	}

	private void Consume(params string[] Images) {
		foreach(string image in Images)
			Consume(image);
	}

	protected Statement Stmt() {
		Token token = LookAhead();

		switch (token.Image) {
			case "{":
				return BlockStmt(token);
			case "var":
				return VarDeclStmt(token);
			case "defer":
				return DeferStmt(token);
			case "render":
				return RenderStmt(token);
			case "if":
				return IfElseStmt(token);
			case "while":
				return WhileStmt(token);
			case "do":
				return DoWhileStmt(token);
			case "return":
				return ReturnStmt(token);
			case "throw":
				return ThrowStmt();
			case "go":
				return GoStmt(token);
			case "switch":
				return SwitchStmt(token);
			case "try":
				return TryCatchFinallyStmt(token);
			case "iter":
				return IterStmt(token);
			case "break":
				return BreakStmt(token);
			case "continue":
				return ContinueStmt(token);
			case "fun":
				return new ExpressionStatement(FuncDeclExpr(NeedsName: true));
			default: {
				ExpressionStatement expressionStatement = new ExpressionStatement();

				while (LookAhead().Image != ";") {
					expressionStatement.Expressions.Add(Expr());
					if(LookAhead().Image != ";")
						Consume(",");
				}

				Consume(";");
				return expressionStatement;
			}
		}
	}

	protected BlockStatement BlockStmt(Token ID) {
		Consume("{");

		BlockStatement blockStatement = new BlockStatement(ID);
		Statement statement = null;

		bool flag = false;
		while(!IsAtEnd() && LookAhead().Image != "}") {
			if(statement != null && !flag && (statement is ReturnStatement || statement is ThrowStatement)) {
				Errors.Add(LookAhead().Locate(), "Unreachable code detected.");
				flag = true;

				continue;
			}

			blockStatement.Statements.Add(statement = Stmt());
			if(IsAtEnd()) {
				Errors.Add(LookAhead().Locate(), "Unclosed block. (" + LookAhead().FileName + ")");
				throw new ParserException();
			}
		}

		Consume("}");
		return blockStatement;
	}

	protected DeferStatement DeferStmt(Token ID) {
		Consume("defer");
		return new DeferStatement(ID, Stmt());
	}

	protected RenderStatement RenderStmt(Token ID) {
		Consume("render");
		RenderStatement result = new RenderStatement(ID, Expr());

		Consume(";");
		return result;
	}

	protected IfElseStatement IfElseStmt(Token ID) {
		Consume("if", "(");

		Expression condition = Expr();
		Consume(")");

		Statement then = Stmt();
		Statement @else = null;

		if(!IsAtEnd() && LookAhead().Image == "else") {
			Consume("else");
			@else = Stmt();
		}

		return new IfElseStatement(ID, condition, then, @else);
	}

	protected WhileStatement WhileStmt(Token ID) {
		Consume("while", "(");
		Expression condition = Expr();

		Consume(")");
		return new WhileStatement(ID, condition, Stmt());
	}

	protected DoWhileStatement DoWhileStmt(Token ID) {
		Consume("do");

		Statement then = Stmt();
		Consume("while", "(");

		Expression condition = Expr();
		Consume(")", ";");

		return new DoWhileStatement(ID, condition, then);
	}

	protected ReturnStatement ReturnStmt(Token ID) {
		Consume("return");
		Expression expr = new LiteralExpression(null);

		if(LookAhead().Image != ";")
			expr = Expr();

		Consume(";");
		return new ReturnStatement(ID, expr);
	}

	protected ThrowStatement ThrowStmt() {
		Consume("throw");
		Expression expression = Expr();

		Consume(";");
		return new ThrowStatement(expression.ID, expression);
	}

	protected GoStatement GoStmt(Token ID) {
		Consume("go");
		return new GoStatement(ID, Stmt());
	}

	protected SwitchStatement SwitchStmt(Token ID) {
		Consume("switch");
		Consume("(");

		Expression condition = Expr();
		List<KeyValuePair<List<Expression>, Statement>> list = new List<KeyValuePair<List<Expression>, Statement>>();

		Statement statement = null;
		Consume(")");
		Consume("{");

		while(!IsAtEnd() && LookAhead().Image != "}") {
			if(IsAtEnd())
				Errors.Add(ID.Locate(), "Unclosed switch statement. (" + ID.FileName + ")");

			Token token = LookAhead();
			if(token.Image == "case") {
				List<Expression> list2 = new List<Expression>();

				while(!IsAtEnd() && LookAhead().Image == "case") {
					Consume("case");
					list2.Add(Expr());

					Consume(":");
				}

				list.Add(new KeyValuePair<List<Expression>, Statement>(list2, Stmt()));
			}
			else if(token.Image == "default") {
				Consume("default");
				if(statement != null)
					Errors.Add(Current().Locate(), "Default label should only be declared once on switch statements. (" + Current().FileName + ")");

				Consume(":");
				statement = Stmt();
			}
			else {
				Errors.Add(Current().Locate(), "Expecting case or default label. (" + Current().FileName + ")");
				Index++;
			}
		}

		Consume("}");
		return new SwitchStatement(ID, condition, list, statement);
	}

	protected TryCatchFinallyStatement TryCatchFinallyStmt(Token ID) {
		Consume("try");

		Statement @try = Stmt();
		Statement @finally = null;
		Token catcher = null;

		Consume("catch");
		if(!IsAtEnd() && LookAhead().Image == "(") {
			Consume("(");
			catcher = Consume(TokenType.IDENTIFIER);
			Consume(")");
		}

		Statement @catch = Stmt();
		if(!IsAtEnd() && LookAhead().Image == "finally") {
			Consume("finally");
			@finally = Stmt();
		}

		return new TryCatchFinallyStatement(ID, @try, catcher, @catch, @finally);
	}

	protected IterStatement IterStmt(Token ID) {
		Consume("iter");
		Consume("(");

		Token placeholder = Consume(TokenType.IDENTIFIER);
		Consume("of");

		Expression array = Expr();
		Consume(")");

		return new IterStatement(ID, placeholder, array, Stmt());
	}

	protected BreakStatement BreakStmt(Token ID) {
		Consume("break", ";");
		return new BreakStatement(ID);
	}

	protected ContinueStatement ContinueStmt(Token ID) {
		Consume("continue", ";");
		return new ContinueStatement(ID);
	}

	protected IncludeStatement IncludeStmt(Token ID) {
		Consume("include");
		List<string> list = new List<string>();

		while(true) {
			list.Add(Consume(TokenType.STRING).Image);

			if(!IsAtEnd() && LookAhead().Image != ";")
				Consume(",");
			else if (LookAhead().Image == ";") break;
		}

		Consume(";");
		return new IncludeStatement(ID, list);
	}

	protected VariableDeclarationStatement VarDeclStmt(Token ID) {
		Token token = LookAhead();
		Consume("var");

		VariableDeclarationStatement variableDeclarationStatement = new VariableDeclarationStatement((ID != null) ? ID : token);
		while(!IsAtEnd() && LookAhead().Image != ";") {
			Token key = Consume(TokenType.IDENTIFIER);
			Expression value = new LiteralExpression(null);

			if(LookAhead().Image == "=") {
				Consume("=");
				value = Expr();
			}

			variableDeclarationStatement.Variables.Add(new KeyValuePair<Token, Expression>(key, value));
			if(LookAhead().Image != ";")
				Consume(",");
		}

		Consume(";");
		return variableDeclarationStatement;
	}

	protected Expression Expr() {
		return LogicExpr();
	}

	protected Expression LogicExpr() {
		Expression expression = NullCoalescingExpr();

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			while (!IsAtEnd() && (image == "&&" || image == "||")) {
				expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), NullCoalescingExpr());

				if (!IsAtEnd())
					image = LookAhead().Image;
			}
		}
		return expression;
	}

	protected Expression NullCoalescingExpr() {
		Expression expression = EqualityExpr();

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			while(!IsAtEnd() && image == "?") {
				expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), EqualityExpr());

				if(!IsAtEnd())
					image = LookAhead().Image;
			}
		}
		return expression;
	}

	protected Expression EqualityExpr() {
		Expression expression = ComparisonExpr();

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			while(!IsAtEnd()) {
				switch(image) {
					case "==":
					case "!=":
					case "=":
						expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), ComparisonExpr());

						if (!IsAtEnd())
							image = LookAhead().Image;
						continue;
				}

				break;
			}
		}

		return expression;
	}

	protected Expression ComparisonExpr() {
		Expression expression = TermExpr();

		if(!IsAtEnd())
			switch (LookAhead().Image) {
				case "<":
				case ">":
				case "<=":
				case ">=":
					expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), TermExpr());
					break;
			}

		return expression;
	}

	protected Expression TermExpr() {
		Expression expression = FactorExpr();

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			while(!IsAtEnd() && (image == "+" || image == "-")) {
				expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), FactorExpr());

				if(!IsAtEnd())
					image = LookAhead().Image;
			}
		}

		return expression;
	}

	protected Expression FactorExpr() {
		Expression expression = UnaryExpr();

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			while(!IsAtEnd()) {
				switch(image) {
					case "/":
					case "*":
					case "%":
						expression = new BinaryExpression(expression, Consume(TokenType.OPERATOR), UnaryExpr());

						if(!IsAtEnd())
							image = LookAhead().Image;
						continue;
				}

				break;
			}
		}

		return expression;
	}

	protected Expression UnaryExpr() {
		if(!IsAtEnd() && LookAhead().Kind == TokenType.OPERATOR && LookAhead(new string[4] { "!", "+", "-", "~" }))
			return new PreOpExpression(Consume(TokenType.OPERATOR), Expr());

		return PrimaryExpr();
	}

	protected Expression ObjectInitExpr() {
		Token token = LookAhead();
		Consume("&");

		if(!IsAtEnd()) {
			string image = LookAhead().Image;

			if(image == "[") {
				Consume("[");
				ArrayExpression arrayExpression = new ArrayExpression();

				while(!IsAtEnd() && !LookAhead().Image.Equals("]")) {
					arrayExpression.Expressions.Add(Expr());

					if(IsAtEnd() || !(LookAhead().Image != "]"))
						break;
					Consume(",");
				}

				Consume("]");
				return arrayExpression;
			}
			if(image == "{") {
				Consume("{");

				List<KeyValuePair<Expression, Expression>> list = new List<KeyValuePair<Expression, Expression>>();
				while(!IsAtEnd() && !LookAhead().Image.Equals("}")) {
					if(LookAhead().Kind == TokenType.IDENTIFIER)
						Errors.Add(LookAhead().Locate(), "Map key cannot be an identifier.");

					Expression key = Expr();
					Expression value = new LiteralExpression(null);

					if(!IsAtEnd() && LookAhead().Image == ":") {
						Consume(":");
						value = Expr();
					}

					list.Add(new KeyValuePair<Expression, Expression>(key, value));
					if(IsAtEnd() || !(LookAhead().Image != "}"))
						break;

					Consume(",");
				}

				Consume("}");
				return new MapExpression(token, list);
			}
		}

		Errors.Add(token.Locate(), "Unexpected token: " + token);
		throw new ParserException();
	}

	protected Expression GroupedExpr() {
		Consume("(");

		GroupedExpression result = new GroupedExpression(Expr());
		Consume(")");

		return result;
	}

	protected Expression PrimaryExpr() {
		Expression expression = null;
		Token token = LookAhead();
		string image = token.Image;

		if(token.Kind == TokenType.STRING)
			expression = new LiteralExpression(Consume(TokenType.STRING).Image);
		else if(token.Kind == TokenType.DIGIT)
			expression = new LiteralExpression(NougatUtil.Digitize(Consume(TokenType.DIGIT).Image));
		else if(image == "fun")
			expression = FuncDeclExpr(NeedsName: false);
		else if(token.Kind == TokenType.IDENTIFIER) {
			expression = new IdentifierExpression(Consume(TokenType.IDENTIFIER));

			if(!IsAtEnd() && LookAhead().Image == ".") {
				List<Token> list = new List<Token>();
				list.Add((expression as IdentifierExpression).ID);

				while(!IsAtEnd() && LookAhead().Image == ".") {
					Consume(".");
					list.Add(Consume(TokenType.IDENTIFIER));
				}

				expression = new SelectExpression(list);
			}
		}
		else
			switch(image) {
				case "true":
					Consume("true");
					expression = new LiteralExpression(true);
					break;
				case "false":
					Consume("false");
					expression = new LiteralExpression(false);
					break;
				case "nil":
					Consume("nil");
					expression = new NilExpression();
					break;
				case "(":
					expression = GroupedExpr();
					break;
				case "&":
					expression = ObjectInitExpr();
					break;
			}
		if(!IsAtEnd() && expression != null) {
			do {
				if(LookAhead().Image == "[")
					while(LookAhead().Image == "[") {
						Consume("[");
						expression = new AccessExpression(expression, Expr());

						Consume("]");
					}

				Token token2 = LookAhead();
				string image2 = token2.Image;

				while(true) {
					if(image2 == "(") {
						Consume("(");
						List<Expression> list2 = new List<Expression>();

						while(true) {
							if(IsAtEnd()) {
								Errors.Add(token2.Locate(), "Unclosed expressional call. (" + token2.FileName + ")");
								throw new ParserException();
							}

							if(LookAhead().Image == ")")
								break;

							list2.Add(Expr());
							if(LookAhead().Image != ")")
								Consume(",");
						}

						Consume(")");
						expression = new CallExpression(expression, list2);
					}

					if (IsAtEnd())
						break;

					if (LookAhead().Image == "(")
						continue;

					goto IL_02f5;
				}

				break;
				IL_02f5:;
			}

			while(LookAhead().Image == "[");
		}

		if(expression == null) {
			Errors.Add(token.Locate(), "Expecting expression. (" + token.FileName + ")");
			throw new ParserException();
		}

		expression.ID = token;
		return expression;
	}

	protected FunctionDeclarationExpression FuncDeclExpr(bool NeedsName) {
		Token token = LookAhead();
		Consume("fun");

		Token name = Token.Create(token.Line, token.Column, TokenType.IDENTIFIER, token.FileName, "<" + token.FileName + ": [" + token.Line + ", " + token.Column + "]>");
		if(NeedsName)
			name = Consume(TokenType.IDENTIFIER);

		Consume("(");
		List<KeyValuePair<Token, Expression>> list = new List<KeyValuePair<Token, Expression>>();

		bool flag = false;
		while (LookAhead().Image != ")" && !(LookAhead().Image == ")")) {
			Token key = Consume(TokenType.IDENTIFIER);
			Expression value = null;

			if(LookAhead().Image == "=") {
				Consume("=");
				value = Expr();

				if(!flag)
					flag = true;
			}
			else if(flag)
				Errors.Add(LookAhead().Locate(), "Expecting expressional value for parameter. (" + LookAhead().FileName + ")");

			list.Add(new KeyValuePair<Token, Expression>(key, value));
			if(LookAhead().Image != ")")
				Consume(",");
		}

		Consume(")");
		return new FunctionDeclarationExpression(name, list, Stmt());
	}

	public override void Run() {
		while(!IsAtEnd()) {
			Token token = LookAhead();

			if(token.Image == "include") {
				Globals.Add(IncludeStmt(token));
				continue;
			}

			if(token.Image == "fun") {
				Globals.Add(new ExpressionStatement(FuncDeclExpr(NeedsName: true)));
				continue;
			}

			if(token.Image == "var") {
				Globals.Add(VarDeclStmt(null));
				continue;
			}

			Errors.Add(Current().Locate(), "Expecting declaration(s). (" + Current().FileName + ")");
			throw new ParserException();
		}
	}

	public override bool IsSuccess() {
		return Errors.Count == 0;
	}
}