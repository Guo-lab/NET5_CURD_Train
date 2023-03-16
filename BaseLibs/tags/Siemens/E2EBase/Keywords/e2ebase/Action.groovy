package e2ebase

import java.lang.annotation.Retention
import java.lang.annotation.RetentionPolicy

@Retention(RetentionPolicy.SOURCE)
public @interface Action {
	String value() default "";
}
