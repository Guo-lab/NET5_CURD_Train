import {  ListInput, } from 'projectbase/projectbase.type';

export interface Task_ListApprovedListVM{
	ResultList: Task_ListApprovedListVM$ListRow[];
	Input: Task_ListApprovedListVM$SearchInput;
}
export interface Task_ListApprovedListVM$ListRow{
	Id: number;
	Code: string;
	Name: string;
	User: any;
	UserName: string;
	MaxItemCount: number;
	CreateDate: Date|null;
	Score: number;
	Status: number;
	Active: boolean;
}
export interface Task_ListApprovedListVM$SearchInput{
	ListInput: ListInput;
}
export interface Task_ListListByDateVM{
	ResultList: Task_ListListByDateVM$ListRow[];
	Input: Task_ListListByDateVM$SearchInput;
}
export interface Task_ListListByDateVM$ListRow{
	Id: number;
	Code: string;
	Name: string;
	User: any;
	UserName: string;
	MaxItemCount: number;
	CreateDate: Date|null;
	Score: number;
	Status: number;
	Active: boolean;
}
export interface Task_ListListByDateVM$SearchInput{
	CreateDate: Date|null;
	ListInput: ListInput;
}
export interface TaskEditVM{
	Input: TaskEditVM$EditInput;
	UserList: any[];
}
export interface TaskEditVM$EditInput{
	Id: number;
	Code: string;
	Name: string;
	User: any;
	MaxItemCount: number;
	CreateDate: Date|null;
	Score: number|null;
	Status: number;
	Active: boolean;
}
export interface TaskListVM{
	ResultList: TaskListVM$ListRow[];
	Input: TaskListVM$SearchInput;
	UserList: any[];
}
export interface TaskListVM$ListRow{
	Id: number;
	Code: string;
	Name: string;
	User: any;
	UserName: string;
	MaxItemCount: number;
	CreateDate: Date|null;
	Score: number;
	Status: number;
	Active: boolean;
	Haha: string;
	Hello: string;
	HelloList: string[];
}
export interface TaskListVM$SearchInput{
	Name: string;
	User: any;
	ListInput: ListInput;
}
export interface TaskMultiEditVM{
	Input: TaskMultiEditVM$MultiEditInput;
	UserList: any[];
	DummyRow: TaskEditVM$EditInput;
}
export interface TaskMultiEditVM$MultiEditInput{
	Rows: TaskEditVM$EditInput[];
}
export interface TaskSectionedInfoVM{
	Section1: TaskSectionedInfoVM$S1;
	Section2: TaskSectionedInfoVM$S2;
	Score: number;
}
export interface TaskSectionedInfoVM$S1{
	Code: string;
	Name: string;
	Section12: TaskSectionedInfoVM$S2;
}
export interface TaskSectionedInfoVM$S2{
	CreateDate: Date|null;
	Score: number;
}
export interface UserEditVM{
	Input: UserEditVM$EditInput;
}
export interface UserEditVM$EditInput{
	Id: number;
	Code: string;
	Name: string;
	Age: number;
	BirthDate: Date|null;
	Salary: number;
	Rank: number;
	Active: boolean;
}
export interface UserListVM{
	ResultList: UserListVM$ListRow[];
	Input: UserListVM$SearchInput;
}
export interface UserListVM$ListRow{
	Id: number;
	Code: string;
	Name: string;
	Age: number;
	BirthDate: Date|null;
	Salary: number;
	Rank: number;
	Active: boolean;
}
export interface UserListVM$SearchInput{
	Name: string;
	ListInput: ListInput;
}
