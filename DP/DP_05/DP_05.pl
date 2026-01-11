man(vova).
man(mike).
man(mark).
man(steve).
man(kevin).
man(max).

man(den).


woman(katya).
woman(dasha).
woman(sarah).
woman(julia).
woman(alice).

woman(liza).

% ШЛЮБИ
married(vova, katya).
married(katya, vova).

married(steve, alice).
married(alice, steve).

married(mike, dasha).
married(dasha, mike).

married(mark, julia).
married(julia, mark).

married(den, liza).
married(liza, den).

% БАТЬКИ
%--
parent(vova, steve).
parent(katya, steve).

parent(steve, kevin).
parent(alice, kevin).

%--
parent(vova, mike).
parent(katya, mike).

parent(mike, mark).
parent(dasha, mark).

parent(mark, max).
parent(julia, max).

%--
parent(vova, sarah).
parent(katya, sarah).

%--
parent(den, dasha).
parent(liza, dasha).

% sibling
sibling(A, B) :-
    parent(P, A),
    parent(P, B),
    A \= B.

% ========================
%   ПРАВИЛА ВАРІАНТУ 13
% ========================

father(F, C) :- man(F), parent(F, C).
mother(M, C) :- woman(M), parent(M, C).

nephew(X) :-
    man(X),
    father(P, X),
    sibling(P, _).

nephew_unique(X) :-
    setof(N, nephew(N), List),
    member(X, List).

father_in_law_of(X, Woman) :-
    woman(Woman),
    married(Woman, Husband),
    father(X, Husband).

father_in_law(X) :-
    setof(FIL, W^father_in_law_of(FIL, W), List),
    member(X, List).

father_in_law_except(X, Woman) :-
    father_in_law_of(X, OtherWoman),
    OtherWoman \= Woman.

?-grandfather_of(_, married(F, P)).

grandfather_of(G, P) :-
    man(G),
    parent(G, X),
    parent(X, P).

cousin_brother(X, Person) :-
    man(X),
    parent(P1, X),
    parent(P2, Person),
    sibling(P1, P2).

married_cousin_brother(X, Person) :-
    cousin_brother(X, Person),
    married(X, _).

has_married_cousin_brother(Person) :-
    married_cousin_brother(_, Person).

print_all_nephews :-
    nephew_unique(X),
    write(X), nl,
    fail.
print_all_nephews.

