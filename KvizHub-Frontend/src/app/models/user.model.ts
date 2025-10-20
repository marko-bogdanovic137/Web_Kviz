export interface User {
  id: number;
  username: string;
  email: string;
  profileImage?: string;
  createdAt: Date;
  isAdmin?: boolean;
}

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  profileImage?: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  email: string;
  profileImage?: string;
  token: string;
  createdAt?: Date;
  isAdmin?: boolean;
}

export interface QuizAdminDTO {
  id: number;
  title: string;
  description: string;
  timeLimit: number;
  difficulty: string;
  isActive: boolean;
  categoryName: string;
  createdByUsername: string;
  questionCount: number;
  createdAt: Date;
}

export interface CreateQuizDTO {
  title: string;
  description: string;
  timeLimit: number;
  difficulty: string;
  categoryId: number;
  questions: CreateQuestionDTO[];
}

export interface CreateQuestionDTO {
  text: string;
  type: string;
  points: number;
  order: number;
  answerOptions: CreateAnswerOptionDTO[];
}

export interface CreateAnswerOptionDTO {
  text: string;
  isCorrect: boolean;
  order: number;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface AdminStatsDTO {
  totalUsers: number;
  totalQuizzes: number;
  totalQuizAttempts: number;
  activeQuizzes: number;
  averageQuizScore: number;
  mostPopularCategory: string;
  categoryStats: CategoryStatsDTO[];
}

export interface CategoryStatsDTO {
  categoryName: string;
  quizCount: number;
  attemptCount: number;
  averageScore: number;
}
export interface QuizDetailDTO {
  id: number;
  title: string;
  description: string;
  timeLimit: number;
  difficulty: string;
  categoryName: string;
  questionCount: number;
  questions: QuestionDetailDTO[];
}

export interface QuestionDetailDTO {
  id: number;
  text: string;
  type: string;
  points: number;
  order: number;
  answerOptions: AnswerOptionDTO[];
}

export interface AnswerOptionDTO {
  id: number;
  text: string;
  isCorrect: boolean;
  order: number;
}