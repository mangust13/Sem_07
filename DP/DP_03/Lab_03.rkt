(define (ВІДОБРАЗИТИ lst func)
  (cond
    ((eq? lst '()) '())
    
    ((pair? lst)
     (cons (func (car lst)) (ВІДОБРАЗИТИ (cdr lst) func)))
    (#t (func lst))))

(define (ОБЕРНЕНИЙ lst)
  (define (helper sublist acc)
    (cond
      ((eq? sublist '()) acc)
      ((pair? sublist)
       (helper (cdr sublist) (cons (car sublist) acc)))
      (#t (cons sublist acc))))
  (helper lst '()))

(define (ОБЕРНЕНИЙ_ДЕКРЕМЕНТ x)
  (cond
    ((eq? x '()) '())
    ((pair? x)
     (cond
       ((pair? (cdr x))
        (ОБЕРНЕНИЙ
         (ВІДОБРАЗИТИ x ОБЕРНЕНИЙ_ДЕКРЕМЕНТ)))
       (#t
        (cons (ОБЕРНЕНИЙ_ДЕКРЕМЕНТ (cdr x))
              (ОБЕРНЕНИЙ_ДЕКРЕМЕНТ (car x))))))
    (#t (- x 1))))

(ОБЕРНЕНИЙ_ДЕКРЕМЕНТ '(1 (4 . 0)))
(ОБЕРНЕНИЙ_ДЕКРЕМЕНТ '(1 4 . 0))