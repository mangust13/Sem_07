:- dynamic family/3.

add_family(Father, Mother, Children) :-
    assertz(family(Father, Mother, Children)).

remove_family(Father, Mother, Children) :-
    retract(family(Father, Mother, Children)).

show_families :-
    forall(
        family(F, M, C),
        ( write(family(F, M, C)), nl )
    ).

income_of_person(person(_, _, _, works(_, Salary)), Salary).
income_of_person(person(_, _, _, unemployed), 0).

sum_incomes([], 0).
sum_incomes([P|T], Sum):-
    income_of_person(P, I),
    sum_incomes(T, Rest),
    Sum is I + Rest.

family_total_income(family(Father, Mother, Children), Total) :-
    income_of_person(Father, IF),
    income_of_person(Mother, IM),
    sum_incomes(Children, IC),
    Total is IF + IM + IC.

working_person(person(_, _, _, works(_, _))).

all_children_working([Child|Rest]) :-
    working_person(Child),
    all_children_working(Rest).
all_children_working([]).

parents_fullnames(person(FName, FSurname, _, _), person(MName, MSurname, _, _),
                 FSurname, FName, MSurname, MName).

variant13(FSurname, FName, MSurname, MName, TotalIncome) :-
    family(Father, Mother, Children),
    Children \= [],
    all_children_working(Children),
    family_total_income(family(Father, Mother, Children), TotalIncome),
    TotalIncome >= 500000,
    parents_fullnames(Father, Mother, FSurname, FName, MSurname, MName).

print_variant13 :-
    variant13(FSurname, FName, MSurname, MName, TotalIncome),
    nl,
    write('Family found:'), nl,
    write('Father: '),
    write(FName), write(' '), write(FSurname), nl,
    write('Mother: '),
    write(MName), write(' '), write(MSurname), nl,
    write('Total family income: '),
    write(TotalIncome), nl,
    nl,
    fail.
print_variant13.


% Fiil data
seed :-
    retractall(family(_,_,_)),
    add_family(
        person(ivan, petrenko, date(7,may,1960), works(plant, 260000)),
        person(olha, petrenko, date(9,may,1961), works(bank, 180000)),
        [
            person(anna, petrenko, date(5,may,2003), works(it, 90000)),
            person(oleh, petrenko, date(1,jan,2006), works(shop, 20000))
        ]
    ),
    add_family(
        person(tom, fox, date(7,may,1960), works(bbc, 15200)),
        person(ann, fox, date(9,may,1961), unemployed),
        [
            person(pat, fox, date(5,may,1983), works(factory, 20000)),
            person(jim, fox, date(5,may,1983), unemployed)
        ]
    ).
