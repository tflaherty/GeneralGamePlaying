%

% xml.pro
% a Prolog to XML translator

% a main entry point just for testing
% purposes.

%%%:- module(amzi_xml).
%%%:- export xml_term/2.

dumpXML(FunctorArityList, XML) :-
	member([Name, Arity], FunctorArityList),	
	%member(Name, FunctorArityElement),
	%member(Arity, FunctorArityElement),
	current_predicate(Name/Arity),
	functor(Pred, Name, Arity),	
	nth_clause(Pred, _, Ref),
    clause(Head, _, Ref),
    xml_term(XML, Head).
    
		
test :-
   mytest(TERM),
   writeq('**********************************'), nl,   
   writeq(TERM), nl,  
   xml_term(XML, TERM),
   writeq(XML), nl,
   %xml_term(XML, TERM2),
   %writeq(TERM2), nl,
   nl,
   fail.
test.

% various test cases

%mytest(hello).
%mytest('Daffy Duck isn''t').
%%%%%%mytest($this is a $$5 string$).
%mytest([23, 2.3, 2.3e4, 2.3e23]).
%mytest([a,b, [c,d, [e,f]]]).
%mytest(X).
%mytest([X,Y,X]).
%mytest(duck(leona)).
%mytest(same(X,X)).
%mytest(different(X,Y)).
%mytest(X = [a,b,foo(2.3, 4)]).
%mytest(complex(first, X = [a,b,foo(2.3, 4)], nest(X, [Y,'a string',X]))).
%mytest(complex(first, X = [a,b,foo(2.3, 4)], nest(X, [Y,'a string',X]))).

% Convert between a Prolog term and a string
% of XML describing that term.  This uses well-formed
% XML, but not 'valid' because it doesn't include
% the DTD specification.  See next predicates for that.

xml_term(XML_STRING, TERM) :-
   var(XML_STRING),
   !,
   term(TERM, _, XML_CHARS, ""),
   string_to_list(XML_STRING, XML_CHARS).
xml_term(XML_STRING, TERM) :-
   string_to_list(XML_STRING, XML_CHARS),
   term(TERM, _, XML_CHARS, ""),
   !.

% A valid XML document has a document specification DTD
% included.  This predicate provides well-formed and
% valid XML.

valid_xml_term(XML_STRING, TERM) :-
   var(XML_STRING),
   !,
   xmldoc(TERM, XML_CHARS, ""),
   string_to_list(XML_STRING, XML_CHARS).
valid_xml_term(XML_STRING, TERM) :-
   string_to_list(XML_STRING, XML_CHARS),
   xmldoc(TERM, XML_CHARS, ""),
   !.

xmlhead -->
   "".

doctype -->
   "",
   "",
   "",
   "",
   "",
   "",
   "",
   "",
   "",
   "",
   "]>".

xmldoc(TERM) -->
   xmlhead,
   doctype,
   "",
   term(TERM, _),
   "".
   
% Check which way we're going.  Need to check
% the XML string, because T might be a variable
% either way.

term(T, VARS, XML, REST) :-
   var(XML),
   !,
   term_to_xml(T, VARS, XML, REST).
term(T, VARS) -->
   xml_to_term(T, VARS).

% These clauses cover the case where the XML
% is provided and we're creating the term.

xml_to_term(T, _) -->
   tag(string),
   !,
   term_codes(CODES),
   endtag(string),
   { string_to_list(T, CODES) }.
xml_to_term(T, _) -->
   tag(atom),
   !,
   term_codes(CODES),
   endtag(atom),
   { atom_codes(T, CODES) }.
xml_to_term(T, _) -->
   tag(number),
   !,
   term_codes(CODES),
   endtag(number),
   { string_to_list(S, CODES), string_term(S, T) }.
xml_to_term(T, VARS) -->
   tag(list),
   !,
   items(T, VARS),
   endtag(list).

%% This used to label this as structure but I've changed it 
%% to 'fact' to work better with the xsl files of games
xml_to_term(T, VARS) -->
   tag(fact),
   !,
   tag(relation),
   term_codes(CODES),
   { atom_codes(NAME, CODES) },
   endtag(relation),
   args(ARGS, VARS),
   { T =.. [NAME|ARGS] },
   endtag(fact).

xml_to_term(T, VARS) -->
   tag(variable),
   !,
   term_codes(CODES),
   { atom_codes(VNAME, CODES), open_member(VNAME=T, VARS) },
   endtag(variable).

% These clauses are for the cases when the
% term is provided and we're generating XML.

%%% This doesn't work for SWI Prolog!
%%%term_to_xml(T, VARS) --> { number(T), ! },
%%%   { string_term(S, T), string_to_list(S, CODES) },
%%%   tag(number),
%%%   term_codes(CODES),
%%%   endtag(number).

term_to_xml(T, _) --> { number(T), ! },
   { number_codes(T, CODES) },
   tag(number),
   term_codes(CODES),
   endtag(number).

term_to_xml(T, _) --> { string(T), ! },
   { string_to_list(T, CODES) },
   tag(string),
   term_codes(CODES),
   endtag(string).
term_to_xml(T, _) --> { atom(T), ! },
   { atom_codes(T, CODES) },
   tag(atom),
   term_codes(CODES),
   endtag(atom).
term_to_xml(T, VARS) --> { is_list(T), ! },
   tag(list),
   items(T, VARS),
   endtag(list).
term_to_xml(T, VARS) --> { structure(T), ! },
   { T =.. [NAME|ARGS], atom_codes(NAME, CODES) },
   tag(fact),
   tag(relation),
   term_codes(CODES),
   endtag(relation),
   args(ARGS, VARS),
   endtag(fact).
   
%%% This doesn't work for SWI Prolog
%%%term_to_xml(T, VARS) --> { var(T), ! },
%%%   tag(variable),
%%%   { string_term(S, T), string_to_list(S, CODES) },
%%%   term_codes(CODES),
%%%   endtag(variable).

term_to_xml(T, _) --> { var(T), ! },
   tag(variable),
   { with_output_to(codes(CODES), write(T)) },
   term_codes(CODES),
   endtag(variable).


% Pick up the value of the list of codes for that
% stuff that resides between two tags.  So the argument
% in these clauses is the list of codes.  The parsed
% input is from the XML string.

term_codes([X|Y]) --> term_code(X), !, term_codes(Y).
term_codes([]) --> [].

% In the code list, that is between the tags, a
% < or > is represented specially.  A real < read
% from the XML string signifies the end of the
% stuff between the tags.  Note this works both
% ways, when the code list is bound or the XML
% text is bound.  The last clause is only in play
% when it's the XML string that is bound.

term_code(0'<) --> "<".
term_code(0'>) --> ">".
term_code(0'&) --> "&".
term_code(X) --> [X], { X \= 0'< }.

% Pick up a tag.

tag(TAG) -->
   white_space, "<", word(TAG), ">".

endtag(TAG) -->
   "</", word(TAG), ">".

solotag(TAG) -->
   "<", word(TAG), "/>".

word(WORD) --> { var(WORD), ! },
   chars(CHARS), { atom_codes(WORD, CHARS) }.
word(WORD) --> { nonvar(WORD) },
   { atom_codes(WORD, CHARS) }, chars(CHARS).

white_space --> white, white_space.
white_space --> [].

white --> [X], { nonvar(X), X =< 32 }.

chars([X|Y]) --> char(X), !, chars(Y).
chars([]) --> [].

char(X) --> [X], { is_char(X) }.

is_char(X) :- X >= 0'a, X =< 0'z, !.
is_char(X) :- X >= 0'A, X =< 0'Z, !.
is_char(X) :- X >= 0'0, X =< 0'9, !.
is_char(0'_).

% Items in a list

items([X|Y], VARS) --> item(X, VARS), !, items(Y, VARS).
items([], _) --> [].

item(X, VARS) -->
   tag(item),
   term(X, VARS),
   endtag(item).

% A list of structure arguments

args([X|Y], VARS) --> sarg(X, VARS), !, args(Y, VARS).
args([], _) --> [].

sarg(X, VARS) -->
   tag(argument),
   term(X, VARS),
   endtag(argument).

% open_member is just like member, but named it this
% just to indicate its working on an open list, and
% adds values that aren't already on the list.

open_member(X, [X|_]).
open_member(X, [_|Y]) :- open_member(X,Y).

string_term(S, T) :- string_to_list(S, L), read_from_chars(L, T).

structure(T) :- compound(T).

% original way to output variable name, need to see if I can use this later
% ?- read_term(Term, [variable_names(VarNames)]), maplist(call, VarNames),
%                                with_output_to(atom(Atom), maplist(write, Term)).
% |: [p(X,Y,Z),r(H,G,K)].
% Term = [p('X', 'Y', 'Z'), r('H', 'G', 'K')],
% VarNames = ['X'='X', 'Y'='Y', 'Z'='Z', 'H'='H', 'G'='G', 'K'='K'],
% Atom = 'p(X,Y,Z)r(H,G,K)'.


%:- end_module(amzi_xml).
