# 1
man(ivan).
man(oleh).
woman(maria).
woman(katya).
woman(olena).

parent(ivan, oleh).
parent(maria, oleh).

parent(ivan, katya).
parent(olena, katya).

parent(oleh, andriy).

half_sibling(X, Y) :- parent(P, X), parent(P, Y), X \= Y,
    (
        (parent(M1, X), parent(M1, Y), parent(F1, X), parent(F2, Y), F1 \= F2)
        ;
        (parent(M1, X), parent(M2, Y), parent(F1, X), parent(F2, Y), M1 \= M2)
    ).

?- half_sibling(X, katya).
?- half_sibling(oleh, katya).
?- half_sibling(X, _).

# 2
man(petro).
man(serhii).
man(taras).

woman(anna).
woman(ivanna).

parent(petro, serhii).
parent(petro, anna).

parent(serhii, maksym).
parent(anna, ivanna).

parent(taras, petro).

uncle(U, Child) :- man(U), parent(P, Child), parent(GP, P), parent(GP, U), U \= P.

?- uncle(U, maksym).
?- uncle(petro, ivan).
?- uncle(_, X).
#aunt(A, Child) :- woman(A), parent(P, Child), parent(GP, P), parent(GP, A), A \= P.

# 3
man(ivan).
woman(iryna).

man(oleh).
woman(olena).

parent(ivan, petro).
parent(iryna, petro).

parent(oleh, taras).
parent(olena, taras).

married(ivan, iryna).
married(iryna, ivan).

married(oleh, olena).
married(olena, oleh).

married_parents(Child, (P1, P2)) :- parent(P1, Child), parent(P2, Child), married(P1, P2).
?- married_parents(X, _).
?- married_parents(petro, X).
?- married_parents(taras, _).

# 4
parent(a, b).
parent(b, c).
parent(c, d).
parent(a, e).

ancestor(X, Y) :- parent(X, Y).
ancestor(X, Y) :- parent(Z, Y), ancestor(X, Z).

# 5
man(ivan).
man(taras).
man(andriy).

woman(iryna).
woman(olha).

parent(iryna, ivan).  
parent(iryna, taras).

parent(olha, andriy).
parent(olha, maryna).

mother_of_son(M) :- woman(M), parent(M, S), man(S).
?- mother_of_son(X).
?- parent(irina, X).
?- mother_of_son(olga).
